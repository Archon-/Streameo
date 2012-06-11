

function successToArtist(result) {

}

var disqus_shortname = 'streameo';
var disqus_developer = 1;
var disqus_identifier = 'StreameoArtistComments' + ArtistName;
var disqus_url = window.location.href + '/Browse/Artist?artist=' + ArtistName;

var ArtistName = "";
function GetArtistName(obj) {
    ArtistName = obj;

    (function () {
        disqus_shortname = 'streameo';
        disqus_developer = 1;
        disqus_identifier = 'StreameoArtistComments' + ArtistName;
        disqus_url = window.location.href + '/Browse/Artist?artist=' + ArtistName;
        
        var dsq = document.createElement('script'); dsq.type = 'text/javascript'; dsq.async = true;
        dsq.src = 'http://streameo.disqus.com/embed.js';
        (document.getElementsByTagName('head')[0] || document.getElementsByTagName('body')[0]).appendChild(dsq);
    })();
}