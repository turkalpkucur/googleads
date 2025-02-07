

function GetAuth() {
    $.ajax({
        type: "get",
        url: "/googleads/GoogleAuth",
        success: function (res) {
      
         
            try {
                window.location.href = res;
            } catch {
                console.log("error-->" + res);
            }
       
        },
      
    });
}