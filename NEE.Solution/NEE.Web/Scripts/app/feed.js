function loadFeed(container, url, maxResults, maxContentLength) {
    if (maxResults === undefined) maxResults = 3;
    if (maxContentLength === undefined) maxContentLength = 250;

    var feedUrl = _js_root_url + 'api/feed?maxContentLength=' + maxContentLength + '&maxResults=' + maxResults + '&feedUrl=' + encodeURIComponent(url);
    var feed = $.get(feedUrl);
    feed.then(function (items) {
        if (items !== undefined && items.length) {                        
            for (var i = 0; i < items.length; i++) {
                var entry = items[i];
                var li = $('<li>' +
                                '<h4><a href="' + entry.link + '" target="_blank">' + entry.title + '</a></h4>' +
                                '<p>' + entry.contentSnippet + '</p>' +
                                '<footer>' + entry.publishedDate + '</footer>' +
                            '</li>');
                container.append(li);
            }
        }            
    });
}
