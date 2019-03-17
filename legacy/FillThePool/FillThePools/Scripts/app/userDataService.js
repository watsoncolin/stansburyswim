var httpVerbs =
    {
    	POST: "POST",
    	PUT: "PUT",
    	GET: "GET",
    	DEL: "DELETE"
    }

var userDataService = (function () {

	var ds =
        {
        	commit: function (type, url, data) {
        		if (type == httpVerbs.POST)
        			delete data["userId"];

        		return $.ajax({
        			type: type,
        			url: url,
        			data: data,
        			dataType: "json"
        		});
        	},
        	save: function (data) {
        		var type = httpVerbs.POST, url = '/api/user';

        		if (data.UserId > 0) {
        			type = httpVerbs.PUT;
        			url += "/" + data.UserId;
        		}

        		return this.commit(type, url, data);
        	},
        	get: function () {
        		return this.commit(httpVerbs.GET, '/api/user/');
        	}
        };

	_.bindAll(ds, "save", "get");

	return {
		save: ds.save,
		get: ds.get
	}
})();