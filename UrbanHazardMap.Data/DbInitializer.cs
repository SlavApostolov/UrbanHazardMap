using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using UrbanHazardMap.Data.Models;

namespace UrbanHazardMap.Data
{
    public class DbInitializer
    {
       public DbInitializer()
       {
        
       }

       
public static async Task SeedDatabase(ApplicationDbContext context)
{

    if (!await context.Institutions.AnyAsync())
    {
        var institutions = SeedInstitutions();

        context.Institutions.AddRange(institutions);

        await context.SaveChangesAsync();
    }

    var existingInstitutions = await context.Institutions.ToListAsync();

    if (!await context.Categories.AnyAsync())
    {
        var categories = SeedCategories(existingInstitutions);

        context.Categories.AddRange(categories);

        await context.SaveChangesAsync();
    }

    await SeedDemoReports(context);
}

public static async Task SeedDemoReports(ApplicationDbContext context)
{
    if (await context.IssueReports.AnyAsync())
    {
        return;
    }

    var categories = await context.Categories.ToListAsync();
    if (categories.Count == 0)
    {
        return;
    }

    Category Find(string title) => categories.First(category => category.Title == title);

    var road = Find("Road Infrastructure");
    var buildings = Find("Buildings and Structures");
    var utilities = Find("Public Utilities");
    var environment = Find("Environmental Hazards");
    var access = Find("Accessibility and Public Safety");

    var reports = new List<IssueReport>
    {
        new IssueReport
        {
            Title = "Pothole in front of NDK",
            Description = "A deep pothole in the bus lane in front of the National Palace of Culture.",
            Latitude = 42.6868,
            Longitude = 23.3187,
            Status = "Approved",
            CreatedAt = DateTime.UtcNow.AddDays(-6),
            Categories = new List<Category> { road }
        },
        new IssueReport
        {
            Title = "Cracked facade near Sofia University",
            Description = "Loose plaster on a building facade above the sidewalk by Sofia University.",
            Latitude = 42.6934,
            Longitude = 23.3348,
            Status = "Approved",
            CreatedAt = DateTime.UtcNow.AddDays(-4),
            Categories = new List<Category> { buildings, access }
        },
        new IssueReport
        {
            Title = "Dark street light at Lion's Bridge",
            Description = "The street light at Lion's Bridge has been out for several nights.",
            Latitude = 42.7046,
            Longitude = 23.3236,
            Status = "Approved",
            CreatedAt = DateTime.UtcNow.AddDays(-3),
            Categories = new List<Category> { utilities }
        },
        new IssueReport
        {
            Title = "Fallen tree in Borisova Gradina",
            Description = "A fallen tree is blocking a footpath in Borisova Gradina.",
            Latitude = 42.6808,
            Longitude = 23.3382,
            Status = "Approved",
            CreatedAt = DateTime.UtcNow.AddDays(-2),
            Categories = new List<Category> { environment, road }
        },
        new IssueReport
        {
            Title = "Blocked ramp at Serdika",
            Description = "The wheelchair ramp by Serdika metro station is blocked by construction barriers.",
            Latitude = 42.6978,
            Longitude = 23.3212,
            Status = "Approved",
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            Categories = new List<Category> { access }
        }
    };

    context.IssueReports.AddRange(reports);
    await context.SaveChangesAsync();
}

public static List<Category> SeedCategories(
    List<Institution> institutions)
{
    return new List<Category>()
    {
        new Category()
        {
            Title = "Road Infrastructure",
            Description = "Damaged roads, potholes, broken sidewalks and unsafe pedestrian crossings.",
            MarkedHexColor = "#E74C3C",
            InstitutionId = institutions
                .First(i => i.Abbreviation == "АПИ").Id
        },

        new Category()
        {
            Title = "Buildings and Structures",
            Description = "Abandoned buildings, structural damage, dangerous facades and collapsing structures.",
            MarkedHexColor = "#E67E22",
            InstitutionId = institutions
                .First(i => i.Abbreviation == "ДНСК").Id
        },

        new Category()
        {
            Title = "Public Utilities",
            Description = "Damaged street lighting, exposed electrical cables, water leaks and sewer problems.",
            MarkedHexColor = "#3498DB",
            InstitutionId = institutions
                .First(i => i.Abbreviation == "КЕВР").Id
        },

        new Category()
        {
            Title = "Environmental Hazards",
            Description = "Illegal dumping, air pollution, contaminated areas and fallen trees.",
            MarkedHexColor = "#27AE60",
            InstitutionId = institutions
                .First(i => i.Abbreviation == "РИОСВ").Id
        },

        new Category()
        {
            Title = "Accessibility and Public Safety",
            Description = "Blocked wheelchair ramps, inaccessible sidewalks, missing safety barriers and other accessibility issues.",
            MarkedHexColor = "#8E44AD",
            InstitutionId = institutions
                .First(i => i.Abbreviation == "АХУ").Id
        }
    };
}

        

        public static List<Institution> SeedInstitutions()
        {
            return new List<Institution>()
            {
                new Institution()
                {
                    Name = "Агенция „Пътна инфраструктура“",
                    Abbreviation= "АПИ",
                    ContactEmail = "info@api.bg",
                    Phone = "0700 130 20"
                },
                new Institution()
                {
                    Name = "Дирекция за национален строителен контрол",
                   Abbreviation = "ДНСК",
                   ContactEmail = "dnsk@dnsk.bg",
                   Phone = "02 9159 137"
                },
                new Institution()
                {
                    Name = "Комисия за енергийно и водно регулиране",
                    Abbreviation = "КЕВР",
                    ContactEmail = "Jalbi_vik@dker.bg",
                    Phone = "02 9359 628"
                },
                new Institution()
                {
                    Name = "Регионална инспекция по околната среда и водите",
                    Abbreviation = "РИОСВ",
                    ContactEmail = "riosv@riew-sofia.org",
                    Phone = "02 940 64 98"
                },
                new Institution()
                {
                    Name = "Агенция за хората с увреждания",
                    Abbreviation= "АХУ",
                    ContactEmail = "ahu@mlsp.government.bg",
                    Phone = "02 931 80 95"
                }
            };
        }
    }
}
