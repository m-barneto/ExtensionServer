setInterval(update, 1000);

String.prototype.format = String.prototype.f = function () {
  var s = this,
    i = arguments.length;

  while (i--) {
    s = s.replace(new RegExp('\\{' + i + '\\}', 'gm'), arguments[i]);
  }
  return s;
};

function update() {
  try {
    let radioName = document.getElementsByClassName("NowPlayingTopInfoSessionName__link")[0].textContent;
    let songNameElement = document.getElementsByClassName("Marquee__wrapper__content")[0];
    if (songNameElement == null) {
      songNameElement = document.getElementsByClassName("Marquee__hiddenSizer")[0];
    }
    let songName = songNameElement.textContent;
    let artistName = document.getElementsByClassName("NowPlayingTopInfo__current__artistName NowPlayingTopInfo__current__artistName--link")[0].textContent;
    let albumName = document.getElementsByClassName("nowPlayingTopInfo__current__albumName nowPlayingTopInfo__current__link")[0].textContent;
    albumName = albumName.replace("(Explicit)", "").replace("(Single)", "").trim();
    let pauseButton = document.getElementsByClassName("PlayButton Tuner__Control__Button Tuner__Control__Play__Button TunerControl")[0].getAttribute("aria-checked");

    let time = document.getElementsByClassName("Duration VolumeDurationControl__Duration")[0];
    let totalTime = time.children[2].textContent;
    let currentTime = time.children[0].textContent;


    // 0:00
    let tTimeString = totalTime.split(':');
    let cTimeString = currentTime.split(':');

    let mins = parseInt(tTimeString[0]) - parseInt(cTimeString[0]);
    let secs = parseInt(tTimeString[1]) - parseInt(cTimeString[1]);
    let epochDiff = (mins * 60) + secs;

    //let albumImage = document.getElementsByClassName("nowPlayingTopInfo__artContainer")[0].children[0].children[0].children[0].getAttribute("src");

    // http://localhost:8080/update?state={0}&details={1}&startTimestamp={2}&endTimestamp={3}&largeImageKey={4}&largeImageText={5}&smallImageKey={6}&smallImageText={7}
    var data = "http://localhost:8080/update?dest=pandorarpc&artist={0}&album={1}&details={2}&startTimestamp={3}&endTimestamp={4}&largeImageKey={5}&largeImageText={6}".f(
      artistName,
      albumName,
      songName,
      0,
      epochDiff,
      "pandora",
      radioName
    );
    var xhr = new XMLHttpRequest();
    // xhr.withCredentials = true;
    xhr.open("POST", data, true);
    xhr.send();
    /*
     *
    $.ajax({
        url: data, success: function (result) {
            alert(result);
        }
    });
    */
  }
  catch (error) {
    console.log(error);
  }
  finally { }
}
