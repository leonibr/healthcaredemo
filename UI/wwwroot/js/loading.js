
(function () {
    /*
     * Loader/splash screen
     * very nice idea from https://github.com/Tewr
     * */

    var total = 0;
    var loaded = 0;
    var progressBar = document.getElementById("pbar");
    var loadingText = document.getElementById("loadingText");
    var proxied = window.fetch;
    window.fetch = function (input, init) {
        var file = input;
        total++;

        return proxied(input, init).then(function (response) {
            loaded++;
            var progress = Math.floor(((loaded / total) * 100));
            progressBar.max = total;
            progressBar.value = loaded;
            loadingText.innerHTML = "Loaded:<br />" + file + ",<br />" + progress + " %..."

            if (loaded == total) {
                // Reset override.
                window.fetch = proxied;
                loadingText.innerHTML = "Loading 100%, opening application...";
            }

            return response;
        });

    };
})();