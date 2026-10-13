(function () {
    var button = document.getElementById("btnGeneratePassword");
    var passwordInput = document.getElementById("txtTemporaryPassword");

    if (!button || !passwordInput) {
        return;
    }

    button.addEventListener("click", function () {
        var chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz23456789!@#$%";
        var password = "";

        for (var i = 0; i < 12; i++) {
            password += chars.charAt(Math.floor(Math.random() * chars.length));
        }

        passwordInput.type = "text";
        passwordInput.value = password;
    });
})();