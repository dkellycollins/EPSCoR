var poll = function (url) {
    var interval = 10000; //10 secs
    setInterval(function () {
        this.load(url);
    }, interval);
}