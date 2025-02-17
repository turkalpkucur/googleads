

function GetAuth() {
    $.ajax({
        type: "get",
        url: "/googleads/GoogleAuth",
        success: function (res) {
            try {
                window.open(
                    res,
                    '_blank'
                );
            } catch {
                console.log("error-->" + res);
            }
        },
    });
}



function GetCustomerListButtonOnClick() {
    let refreshToken = document.getElementById('ReturnRefreshTokenId').innerHTML;
    $.ajax({
        type: "POST",
        data: { refreshToken: refreshToken },
        url: "/googleads/ListAvaiableCustomers",
        success: function (res) {
            $("#ResultDivId").show();
            if (res.length > 0) {
                $("#ResultDivId").append("<ul>");
                for (let i = 0; i < res.length; i++) {
                    $("#ResultDivId").append("<li>");
                    $("#ResultDivId").append(res[i]);
                    $("#ResultDivId").append("</li>");
                }
                $("#ResultDivId").append("</ul>");
            }
        },
    });
}

function UploadOfflineConversionOnClick() {
    let conversionValue = $("#ConversionValueId").val();
 
    let conversionActionId = $("#ConversionActionId").val();

    let gclId = $("#GclId").val();
    let gbraId = $("#GbraId").val();
    let wbraId = $("#WbraId").val();
    let refreshToken = document.getElementById('ReturnRefreshTokenId').innerHTML;

    $.ajax({
        type: "POST",
        url: "/googleads/UploadOfflineConversion",
        data: { refreshToken: refreshToken, conversionValue: conversionValue,   conversionActionId: conversionActionId, gclId: gclId, gbraId: gbraId, wbraId: wbraId  },
        success: function () {
            debugger;
        }
    });
}

