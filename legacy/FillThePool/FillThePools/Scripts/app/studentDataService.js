var httpVerbs =
    {
    	POST: "POST",
    	PUT: "PUT",
    	GET: "GET",
    	DEL: "DELETE"
    }

var studentDataService = (function () {

	var ds =
        {
        	commit: function (type, url, data) {
        		if (type == httpVerbs.POST)
        			delete data["studentId"];

        		return $.ajax({
        			type: type,
        			url: url,
        			data: data,
        			dataType: "json"
        		});
        	},
        	del: function (data) {
        		return this.commit(httpVerbs.DEL, '/api/student/' + data.studentId);
        	},
        	save: function (data) {
        		var type = httpVerbs.POST, url = '/api/student';

        		if (data.studentId > 0) {
        			type = httpVerbs.PUT;
        			url += "/" + data.studentId;
        		}

        		return this.commit(type, url, data);
        	},
        	get: function () {
        		return this.commit(httpVerbs.GET, '/api/student/');
        	}
        };

	_.bindAll(ds, "del", "save", "get");

	return {
		save: ds.save,
		del: ds.del,
		get: ds.get
	}
})();