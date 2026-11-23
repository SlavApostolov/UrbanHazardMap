
using System.Globalization;
using System.Net;
using System.Text;
using UrbanHazardMap.Data.Models;

namespace UrbanHazardMap.Services
{
    public static class ReportNotificationEmailBuilder
    {
        public static string BuildBody(
            IssueReport report,
            IEnumerable<Category> institutionCategories)
        {
            var title = WebUtility.HtmlEncode(report.Title);
            var description = WebUtility.HtmlEncode(report.Description)
                .Replace("\r\n", "\n")
                .Replace("\n", "<br>");

            var latitude = report.Latitude.ToString(
                CultureInfo.InvariantCulture);

            var longitude = report.Longitude.ToString(
                CultureInfo.InvariantCulture);

            var mapUrl =
                $"https://www.openstreetmap.org/?mlat={latitude}&mlon={longitude}#map=17/{latitude}/{longitude}";

            var encodedMapUrl = WebUtility.HtmlEncode(mapUrl);

            var createdAt = report.CreatedAt
                .ToUniversalTime()
                .ToString("dd MMMM yyyy, HH:mm", CultureInfo.InvariantCulture);

            var categoriesHtml = new StringBuilder();

            foreach (var category in institutionCategories)
            {
                categoriesHtml.Append($@"
                    <span style='
                        display:inline-block;
                        background-color:#EFF6FF;
                        color:#1D4ED8;
                        padding:8px 12px;
                        margin:0 6px 8px 0;
                        border-radius:6px;
                        font-size:13px;
                        font-weight:600;'>
                        {WebUtility.HtmlEncode(category.Title)}
                    </span>");
            }

            return $@"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>UrbanHazardMap Notification</title>
</head>

<body style='
    margin:0;
    padding:0;
    background-color:#F3F6FA;
    font-family:Arial,Helvetica,sans-serif;
    color:#334155;'>

    <table role='presentation'
           width='100%'
           cellpadding='0'
           cellspacing='0'
           style='background-color:#F3F6FA;'>
        <tr>
            <td align='center' style='padding:32px 16px;'>

                <table role='presentation'
                       width='600'
                       cellpadding='0'
                       cellspacing='0'
                       style='
                           width:100%;
                           max-width:600px;
                           background-color:#FFFFFF;
                           border:1px solid #E5EAF0;'>

                    <!-- HEADER -->
                    <tr>
                        <td style='
                            background-color:#163A5F;
                            padding:28px 32px;'>

                            <h1 style='
                                margin:0;
                                color:#FFFFFF;
                                font-size:26px;
                                line-height:32px;'>
                                UrbanHazardMap
                            </h1>

                            <p style='
                                margin:8px 0 0;
                                color:#D9EAF7;
                                font-size:12px;
                                letter-spacing:1px;'>
                                CITY INFRASTRUCTURE REPORTING SYSTEM
                            </p>
                        </td>
                    </tr>

                    <!-- CONTENT -->
                    <tr>
                        <td style='padding:32px;'>

                            <p style='margin:0 0 20px;'>
                                <span style='
                                    background-color:#E7F5EC;
                                    color:#197044;
                                    padding:7px 12px;
                                    font-size:12px;
                                    font-weight:bold;'>
                                    APPROVED REPORT
                                </span>
                            </p>

                            <h2 style='
                                color:#163A5F;
                                font-size:24px;
                                line-height:32px;
                                margin:0 0 16px;'>
                                {title}
                            </h2>

                            <p style='
                                font-size:15px;
                                line-height:24px;
                                margin:0 0 28px;'>
                                A new urban hazard report has been approved
                                and requires your institution's attention.
                            </p>

                            <hr style='
                                border:0;
                                border-top:1px solid #E5EAF0;
                                margin:0 0 24px;'>

                            <p style='
                                color:#64748B;
                                font-size:12px;
                                font-weight:bold;
                                letter-spacing:1px;
                                margin:0 0 10px;'>
                                DESCRIPTION
                            </p>

                            <p style='
                                font-size:15px;
                                line-height:24px;
                                margin:0 0 28px;
                                overflow-wrap:break-word;'>
                                {description}
                            </p>

                            <p style='
                                color:#64748B;
                                font-size:12px;
                                font-weight:bold;
                                letter-spacing:1px;
                                margin:0 0 12px;'>
                                RELEVANT CATEGORIES
                            </p>

                            <div style='margin-bottom:24px;'>
                                {categoriesHtml}
                            </div>

                            <p style='
                                color:#64748B;
                                font-size:12px;
                                font-weight:bold;
                                letter-spacing:1px;
                                margin:0 0 8px;'>
                                REPORT CREATED
                            </p>

                            <p style='
                                font-size:15px;
                                margin:0 0 30px;'>
                                {createdAt} UTC
                            </p>

                            <!-- MAP BUTTON -->
                            <table role='presentation'
                                   cellpadding='0'
                                   cellspacing='0'>
                                <tr>
                                    <td bgcolor='#2563EB'
                                        style='
                                            background-color:#2563EB;
                                            padding:14px 24px;'>

                                        <a href='{encodedMapUrl}'
                                           style='
                                               color:#FFFFFF;
                                               text-decoration:none;
                                               font-size:15px;
                                               font-weight:bold;
                                               display:inline-block;'>
                                            View Location on Map
                                        </a>
                                    </td>
                                </tr>
                            </table>

                            <p style='
                                font-size:12px;
                                line-height:19px;
                                color:#64748B;
                                margin:16px 0 0;'>
                                Location: {latitude}, {longitude}
                            </p>

                        </td>
                    </tr>

                    <!-- FOOTER -->
                    <tr>
                        <td style='
                            background-color:#F8FAFC;
                            border-top:1px solid #E5EAF0;
                            padding:24px 32px;'>

                            <p style='
                                margin:0 0 8px;
                                font-size:13px;
                                color:#475569;'>
                                This is an automated notification
                                from UrbanHazardMap.
                            </p>

                            <p style='
                                margin:0;
                                font-size:12px;
                                line-height:19px;
                                color:#64748B;'>
                                The report photo is attached when available.
                                Please do not reply to this automated message.
                            </p>

                        </td>
                    </tr>

                </table>

            </td>
        </tr>
    </table>

</body>
</html>";
        }
    }
}
