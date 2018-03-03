define(
  ["sitecore",
    "/-/speak/v1/experienceprofile/DataProviderHelper.js",
    "/-/speak/v1/experienceprofile/CintelUtl.js"
  ],
  function (sc, providerHelper, cintelUtil, ExternalDataApiVersion) {
    var cidParam = "cid";
    var intelPath = "/intel";
 
    var app = sc.Definitions.App.extend({
      initialized: function () {
		
		
		$("body").on('click','.facets-collection .btn',function(){
			var $facet = $(this).closest('.control-form-user-editor');
			var facetName = $facet.data('facet-name');
			var container = $facet.data('container');
			var fieldName = $facet.data('field-name');
			var value = $facet.find('.facet-value').val();
			debugger;
			$.ajax({
            type: "POST",
			
            url: '/api/sitecore/XEditor/EditorFacet',
            data: JSON.stringify({facetName:facetName,container:container,fieldName:fieldName, contactId:contactId, value:value }),
            cache: false,
             contentType: "application/json; charset=utf-8",
			dataType: "json",
            processData: false,
            success: function(data){
                if (data) {
                    alert(data);
                }
            }
        });
		});
		
		
        $('.sc-progressindicator').first().show().hide();
        var contactId = cintelUtil.getQueryParam(cidParam);
        var tableName = "";
        var baseUrl = "/sitecore/api/ao/v1/contacts/" + contactId + "/intel/facet";
 
        providerHelper.initProvider(this.FacetsDataProvider,
          tableName,
          baseUrl,
          this.ExternalDataTabMessageBar);
 
        providerHelper.getData(this.FacetsDataProvider,
          $.proxy(function (jsonData) {
            var dataSetProperty = "Data";
			var $container = $("<div class='facets-collection form-user-editor' />");
			
			var facetName = "";

			var $style = $("<style>.form-user-editor .control-form-user-editor input.btn{background-color: #d1d1d1; margin-left: 10px; width: 50px; padding-left: 10px;} label { display: inline-block; width: 150px; text-align: right; } input { width: 300px; box-sizing: border-box; border: 1px solid #999; } input:focus, textarea:focus { /* To give a little highlight on active elements */ border-color: #000; } .form-user-editor .control-form-user-editor label { display: block; float: left; width: 175px; color: #707070; clear: both; font-weight: normal; margin-top: 8px; text-align: left; } .form-user-editor .control-form-user-editor input { height: 36px; padding: 8px 12px; font-size: 12px; line-height: 1.42857143; color: #474747; background-color: #fff; background-image: none; border: 1px solid #cccccc; border-radius: 2px; -webkit-box-shadow: inset 0 1px 1px rgba(0, 0, 0, 0.075); box-shadow: inset 0 1px 1px rgba(0, 0, 0, 0.075); -webkit-transition: border-color ease-in-out .15s, box-shadow ease-in-out .15s; -o-transition: border-color ease-in-out .15s, box-shadow ease-in-out .15s; transition: border-color ease-in-out .15s, box-shadow ease-in-out .15s; } .form-user-editor .control-form-user-editor input:focus { border-color: #66afe9; outline: 0; -webkit-box-shadow: inset 0 1px 1px rgba(0,0,0,.075), 0 0 8px rgba(102, 175, 233, 0.6); box-shadow: inset 0 1px 1px rgba(0,0,0,.075), 0 0 8px rgba(102, 175, 233, 0.6); }</style>");
			$container.append($style);
			
            if (jsonData.data.dataSet != null && jsonData.data.dataSet.facet.length > 0) {
			  $.each(jsonData.data.dataSet.facet, function(){
				  var dataSet = this;
				  var cssClass = dataSet.DepthLevel=="0" 
				  ? "" 
				  : dataSet.DepthLevel=="1" 
						? "padding-left: 100px;"
						: "padding-left: 200px;";
				  var $html = $("<div class='control-form-user-editor' style='margin-top: 1em; "+cssClass+"' data-facet-name='"+dataSet.FacetName +"' data-container='"+dataSet.Type+"' data-field-name='"+dataSet.FieldName+"'  />");
				  if(facetName!==dataSet.FacetName){
					  if(cssClass!=""){
						  $html.append($("<h4>"+dataSet.FacetName+"</h4>"));
					  }
					  else{
						  $html.append($("<h3>"+dataSet.FacetName+"</h3>"));
					  }
					  facetName = dataSet.FacetName;
				  }
				  if(dataSet.FieldValue!=='$object$'){
					  var $input = "<input type='text' class='facet-value' style='width:200px;' value='"+dataSet.FieldValue+"' />"
					  var $button = "<input type='button' class='btn' value='Save' />";
					  $html.append("<label>"+dataSet.FieldName+ "</label>"+$input+$button);
				  }
				  else{
					  $html.append("<label>"+dataSet.FieldName+ "</label>");
				  }
				  

				  $container.append($html);
			  });
				
              
			 this.FacetsDataProvider.set(dataSetProperty, jsonData);
			 this.FacetsIdBorder.viewModel.$el.append($container);

            } else {
              this.ExternalDataTabMessageBar.addMessage("notification", this.NoFacetData.get("text"));
            }
          }, this));
      }
    });
    return app;
  });
  