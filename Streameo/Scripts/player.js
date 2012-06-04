$(document).ready(function () {
    LoadMusicFile(1);
});

$('.load-song').live('click', function (e) {
    e.preventDefault();
});

var myPlaylist;
var title1 = "error";
function LoadMusicFile(fileId, param) {
    if (param) {
        $.ajax(
                {
                    type: "POST",
                    url: "/Player/ListenData/" + fileId,
                    success:
                        function (data) {
                            var array = data.split("!TitleArtistSeparator!");

                            myPlaylist.setPlaylist([
                              {
                                  title: array[0],
                                  artist: array[1],
                                  mp3: "/Player/ListenFile/" + fileId
                              }
                            ]);
                            myPlaylist.play();
                        }
                });
    }
    else {
        $.ajax(
                {
                    type: "POST",
                    url: "/Player/ListenData/" + fileId,
                    success:
                        function (data) {
                            var array = data.split("!TitleArtistSeparator!");

                            myPlaylist = new jPlayerPlaylist({
                                jPlayer: "#jquery_jplayer_1",
                                cssSelectorAncestor: "#jp_container_1"
                            }, [
                              {
                                  title: array[0],
                                  artist: array[1],
                                  mp3: "/Player/ListenFile/" + fileId
                              }
                            ], {
                                playlistOptions: {
                                    autoPlay: true,
                                    enableRemoveControls: true
                                },
                                swfPath: "Swf/",
                                supplied: "mp3"
                            });
                        }
                });
    }
}

function AddToPlaylist(fileId) {
    $.ajax(
                {
                    type: "POST",
                    url: "/Player/ListenData/" + fileId,
                    success:
                        function (data) {
                            var array = data.split("!TitleArtistSeparator!");
                            myPlaylist.add({
                                title: array[0],
                                artist: array[1],
                                mp3: "/Player/ListenFile/" + fileId
                            });
                        }
                });
}