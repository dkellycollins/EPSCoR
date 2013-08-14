dojo.require("dijit.layout.BorderContainer");
dojo.require("dijit.layout.ContentPane");
dojo.require("dijit.layout.AccordionContainer");
dojo.require("dijit.TitlePane");
dojo.require("dojo.parser");
dojo.require("esri.arcgis.utils");
dojo.require("esri.dijit.BasemapGallery");
dojo.require("esri.dijit.Legend");
dojo.require("esri.dijit.Scalebar");
dojo.require("esri.layers.FeatureLayer");
dojo.require("esri.map");
dojo.require("esri.virtualearth.VETiledLayer");

var map;

function init() {
    map = new esri.Map("map", {
        basemap: "topo",
        center: [-96.53, 38.374],
        zoom: 13
    });

    var rivers = new esri.layers.FeatureLayer("http://sampleserver3.arcgisonline.com/ArcGIS/rest/services/Hydrography/Watershed173811/MapServer/1", {
        mode: esri.layers.FeatureLayer.MODE_ONDEMAND,
        outFields: ["*"]
    });
    var waterbodies = new esri.layers.FeatureLayer("http://sampleserver3.arcgisonline.com/ArcGIS/rest/services/Hydrography/Watershed173811/MapServer/0", {
        mode: esri.layers.FeatureLayer.MODE_ONDEMAND,
        outFields: ["*"]
    });

    //add the legend
    dojo.connect(map, 'onLayersAddResult', function (results) {
        var layerInfo = dojo.map(results, function (layer, index) {
            return { layer: layer.layer, title: layer.layer.name };
        });
        if (layerInfo.length > 0) {
            var legendDijit = new esri.dijit.Legend({
                map: map,
                layerInfos: layerInfo
            }, "legendDiv");
            legendDijit.startup();

            //add the basemap gallery, in this case we'll display maps from ArcGIS.com including bing maps
            var basemapGallery = new esri.dijit.BasemapGallery({
                showArcGISBasemaps: true,
                map: map
            }, "basemapGallery");
            basemapGallery.startup();

            var scalebar = new esri.dijit.Scalebar({
                map: map,
                // "dual" displays both miles and kilmometers
                // "english" is the default, which displays miles
                // use "metric" for kilometers
                scalebarUnit: "dual",
                attachTo: "bottom-left"
            });
        }
    });

    map.addLayers([waterbodies, rivers]);


    dojo.connect(map, 'onLoad', function (theMap) {
        //resize the map when the browser resizes
        dojo.connect(dijit.byId('map'), 'resize', map, map.resize);
    });
}

dojo.ready(init);