using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using UrbanHazardMap.Models;

namespace UrbanHazardMap.Data
{
    public class DbInitializer
    {
       public DbInitializer()
       {
        
       }

       
public static async Task SeedDatabase(ApplicationDbContext context)
{
    // STEP 1: Seed institutions if they don't exist

    if (!await context.Institutions.AnyAsync())
    {
        var institutions = SeedInstitutions();

        context.Institutions.AddRange(institutions);

        await context.SaveChangesAsync();
    }

    // STEP 2: Retrieve institutions from the database

    var existingInstitutions = await context.Institutions.ToListAsync();

    // STEP 3: Seed categories if they don't exist

    if (!await context.Categories.AnyAsync())
    {
        var categories = SeedCategories(existingInstitutions);

        context.Categories.AddRange(categories);

        await context.SaveChangesAsync();
    }
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
