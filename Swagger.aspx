<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Swagger.aspx.cs" Inherits="cdn.Swagger" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <!-- Add Swagger UI CSS -->
    <link rel="icon" type="image/png" href="swagger-ui/favicon-32x32.png" sizes="32x32" />
    <link rel="icon" type="image/png" href="swagger-ui/favicon-16x16.png" sizes="16x16" />
    <link rel="stylesheet" type="text/css" href="swagger-ui/swagger-ui.css" />
    <title></title>
</head>
<body>
    <div id="swagger-ui"></div>
    <script src="swagger-ui/swagger-ui-bundle.js"></script>
    <script src="swagger-ui/swagger-ui-standalone-preset.js"> </script>
    <script>
        <%
        var swaggerFile = "swagger.json?v=2";
        var user = (string)Request.QueryString["u"];
        switch (user)
        {
            case "ad": swaggerFile = "swagger_admin.json"; break;
            case "su": swaggerFile = "swagger_support.json"; break;
        }
        %>
        window.onload = function () {
            const ui = SwaggerUIBundle({
                url: "<%=swaggerFile%>",
                dom_id: "#swagger-ui",
                deepLinking: true,
                presets: [
                    SwaggerUIBundle.presets.apis,
                    SwaggerUIBundle.SwaggerUIStandalonePreset
                ],
                plugins: [
                    SwaggerUIBundle.plugins.DownloadUrl,
                    SwaggerUIBundle.plugins.Auth
                ],
            });
        };
    </script>
</body>
</html>
