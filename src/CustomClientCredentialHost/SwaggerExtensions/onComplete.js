$('#input_apiKey').change(function () {
    var key = $('#input_apiKey')[0].value;
    var credentials = key.split(':'); //clientid:clientsecret:scope expected
    console.log(credentials);
    $.ajax({
        url: "/idsrv3core/connect/token",
        type: "post",
        contenttype: 'x-www-form-urlencoded',
        data: "grant_type=client_credentials&client_id=" + credentials[0] + "&client_secret=" + credentials[1] + "&scope=" + credentials[2],
        success: function (response) {
            var bearerToken = 'Bearer ' + response.access_token;
            console.log(bearerToken);
            var apiKeyAuth = new window.SwaggerClient.ApiKeyAuthorization("Authorization", bearerToken, "header");
            window.swaggerUi.api.clientAuthorizations.add("token", apiKeyAuth);
        },
        error: function (xhr, ajaxoptions, thrownerror) {
            alert("Login failed!");
        }
    });
});