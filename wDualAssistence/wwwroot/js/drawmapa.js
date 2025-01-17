 var colors= ['FF6600', 'FF0000', '003366', '009933', '996633', '6CE0FF', 
                   '3C3EFF', '800000', '30D585', '003300', 'FFFF66', '808080', '7A995C', 
                   'silver', '522900', '0000FF', '490000'];
function crearPolygono(cordenadas,nombreruta ,mapa, editable,colorLinea,colorFondo,trasparentLinea,trasparentFondo) {
	if(cordenadas.length>0){
	var poly = new google.maps.Polygon({
		paths : cordenadas,
		strokeColor : colorLinea,
		strokeOpacity : trasparentLinea,
		editable : editable,
		draggable : editable,
		strokeWeight : 3,
		fillColor : colorFondo,
		fillOpacity : trasparentFondo,
		description: nombreruta
	    
	    
	});
	
	google.maps.event.addListener(poly, 'click', function(event) {
		  alert(this.description);
	});

	poly.setMap(mapa);
	return poly;
	}
	return null;
}

function crearPolygono_idZona(cordenadas, ruta ,mapa, editable,colorLinea,colorFondo,trasparentLinea,trasparentFondo) {
	if(cordenadas.length>0){
	var poly = new google.maps.Polygon({
		paths : cordenadas,
		strokeColor : colorLinea,
		strokeOpacity : trasparentLinea,
		editable : editable,
		draggable : editable,
		strokeWeight : 3,
		fillColor : colorFondo,
		fillOpacity : trasparentFondo,
		ruta:ruta
	    
	    
	});
	
//	google.maps.event.addListener(poly, 'click', function(event) {
//		  alert(this.description);
//	});

	poly.setMap(mapa);
	return poly;
	}
	return null;
}


function crearPolygonoS(cordenadas ,mapa, editable,colorLinea,colorFondo,trasparentLinea,trasparentFondo) {
	if(cordenadas.length>0){
	var poly = new google.maps.Polygon({
		paths : cordenadas,
		strokeColor : colorLinea,
		strokeOpacity : trasparentLinea,
		editable : editable,
		draggable : editable,
		strokeWeight : 3,
		fillColor : colorFondo,
		fillOpacity : trasparentFondo
		
	    
	    
	});
	

	poly.setMap(mapa);
	return poly;
	}
	return null;
}

function get_random_color() 
{
    var color = "";
    for(var i = 0; i < 3; i++) {
    	var sub = Math.floor(Math.random() * 256).toString(16);
    	color += (sub.length == 1 ? "0" + sub : sub);
    }
    return "#" + color;
}

function dibujarZonas(listZona, mapa) {
	var listPolygono=[];
	if(listZona){
		for ( var i = 0; i < listZona.length; i++) {
//			var color=colors.getRandom();
			var color=get_random_color();
//			var poly=crearPolygono(getCoordenadasZona(listZona[i]), mapa, false, color, color, 0.3, 0.3);
			var nombre=getNombreRuta(listZona[i]);
			var poly=crearPolygono(getCoordenadasGoogle(listZona[i]),nombre, mapa, false, color, color, 0.6, 0.6);
			
			listPolygono.push(poly);
		}
		return listPolygono;
	}else{
		return null;
	}
}
function dibujarPolygonosDeRutas(listZona, mapa,colorLinea,colorFondo,transparenciaLinea,transparenciaFondo) {
	var listPolygono=[];
	if(listZona){
		for ( var i = 0; i < listZona.length; i++) {
//			var color=colors.getRandom();
			
//			var poly=crearPolygono(getCoordenadasZona(listZona[i]), mapa, false, color, color, 0.3, 0.3);
			var nombre=getNombreRuta(listZona[i]);
			var poly=crearPolygono(getCoordenadasGoogle(listZona[i]),nombre, mapa, false, colorLinea, colorFondo, transparenciaLinea, transparenciaFondo);
			
			listPolygono.push(poly);
		}
		return listPolygono;
	}else{
		return null;
	}
}
function getCoordenadasZona(zona){
	var liscoor = [];
	if(zona.listaCoordenadasZona){
		for ( var i = 0; i < zona.listaCoordenadasZona.length; i++) {
			liscoor.push(new google.maps.LatLng(zona.listaCoordenadasZona[i].latitud, zona.listaCoordenadasZona[i].longitud));
		}
	
	}
	return liscoor;
}

function crearCircle(cordenada, mapa, editable,colorLinea,colorFondo,trasparentLinea,trasparentFondo,radio) {
	
	var circleOptions = {
		strokeColor : colorLinea,
		strokeOpacity : trasparentLinea,
		strokeWeight : 2,
		fillColor : colorFondo,
		fillOpacity : trasparentFondo/100,
		editable : editable,
		draggable : editable,
		map: mapa,
	    center: cordenada,
	    zIndex:10,
	    radius: radio
	};
	var circle=new google.maps.Circle(circleOptions);
	return circle;
}
function crearPolylinea(mapa) {
	var polyOptions = {
		strokeColor : '#000000',
		strokeOpacity : 1.0,
		editable : true,
		strokeWeight : 3
	};
	var poly = new google.maps.Polyline(polyOptions);
	poly.setMap(mapa);

	return poly;
}
function eliminarPolylinea(polylinea) {
	polylinea.setMap(null);
}


function eliminarPolygonos(listPoly) {
	for ( var i = 0; i < listPoly.length; i++) {
		listPoly[i].setMap(null);
		
	}
}
function eliminarPolygono(poly) {
	poly.setMap(null);
}
Array.prototype.getRandom= function(){
    var i= Math.floor(Math.random()*this.length);
    
    if( i in this){
	    
        return this.splice(i, 1)[0];
    }
    return Math.random().toString(16).substr(2,6);
};

/**
 * @param list lista de Json [{lat,lng},...]
 * @returns {Array} array de coordenads de google maps LatLong
 */
function getNombreRuta(list){
	var nombre=list[0].nombreRuta;
	
	return nombre;
}
function getCoordenadasRutasGoogle(list){
	var liscoor = [];

	for ( var i = 0; i < list.length; i++) {		
		if(list[i].latitud!=0.0 &&  list[i].longitud!=0.0){
			liscoor.push(new google.maps.LatLng(list[i].latitud, list[i].longitud));
		}
		
	}
	return liscoor;
	
}
function getCoordenadasRutas(list){
	var liscoor = [];

	for ( var i = 0; i < list.length; i++) {		
		liscoor.push(new google.maps.LatLng(list[i].latitud, list[i].longitud));
	}
	return liscoor;
	
}
function getCoordenadasGoogle(list) {
	var liscoor = [];

	for ( var i = 0; i < list.length; i++) {		
		liscoor.push(new google.maps.LatLng(list[i].lat, list[i].lng));
	}
	return liscoor;
}

/**
 * @param list
 *            Array en MVC
 * @returns {Array} coordenadas en google LatLng
 */
function mvcToCoorGoogle(list) {
	var liscoor = [];

	for ( var i = 0; i < list.getLength(); i++) {
		liscoor.push(list.getAt(i));
	}
	return liscoor;
}
/**
 * @param list
 *            Array en MVC
 * @returns {string} coordenadas json {lat,lng}
 */
function mvcToString(list) {
	var liscoor="";
	if (list.getLength() > 0) {
		liscoor = "[";
		var i;
		for ( i=0; i < list.getLength()-1; i++) {
			liscoor+='{"lat":' + list.getAt(i).lat() + ',' + '"lng":'
					+ list.getAt(i).lng()+'},';
			
		}
		liscoor+='{"lat":' + list.getAt(i).lat() + ',' + '"lng":'
				+ list.getAt(i).lng()+'}';
		liscoor+="]";
	}
	return liscoor;

}

/**
 * @param poly Adicionar eventis de dragend,insertar-modificar-remover punto, polylinea o polygono
 */
function addEventoPolygono(poly) {
	google.maps.event.addListener(poly, 'dragend', actualizarCoordenadas);
	google.maps.event.addListener(poly.getPath(), 'insert_at',
			actualizarCoordenadas);
	google.maps.event.addListener(poly.getPath(), 'remove_at',
			actualizarCoordenadas);
	google.maps.event.addListener(poly.getPath(), 'set_at',
			actualizarCoordenadas);
		
}



function addEventoClick(poly){
	google.maps.event.addListener(poly, 'click', function(event) {
		  alert(this.description);
	});
}

function getCoordenadasRutasGoogle(list){
	var liscoor = [];

	for ( var i = 0; i < list.length; i++) {		
		if(list[i].latitud!=0.0 &&  list[i].longitud!=0.0){
			liscoor.push(new google.maps.LatLng(list[i].latitud, list[i].longitud));
			
		}
		
	}
	
	return liscoor;
	
}
function getCoordenadasGoogle_ClienteRuta(list){
	var liscoor = [];

	for ( var i = 0; i < list.length; i++) {		
		if(list[i].cliente.latitud!=0 &&  list[i].cliente.longitud!=0){
			liscoor.push(new google.maps.LatLng(list[i].cliente.latitud, list[i].cliente.longitud));
		}
		
	}
	return liscoor;
	
}
