/*!
  SerializeJSON jQuery plugin.
  https://github.com/marioizquierdo/jquery.serializeJSON
  version 2.9.0 (Jan, 2018)

  Copyright (c) 2012-2018 Mario Izquierdo
  Dual licensed under the MIT (http://www.opensource.org/licenses/mit-license.php)
  and GPL (http://www.opensource.org/licenses/gpl-license.php) licenses.
*/

function Message(titulo, mensaje) {
    var toastObj = document.getElementById('element').ej2_instances[0];
    toastObj.title = titulo;
    toastObj.content = mensaje;
    toastObj.show();
}

// ********************************************************************* Javascript para spc ****************************
function cambioHaPropias() {
    document.getElementById('spc_RendimientoActual.ha_propias').value = document.getElementById('spc_Identificacion.ha_propias').value;
}

function cambioHaAlquiladas() {
    document.getElementById('spc_RendimientoActual.ha_alquiladas').value = document.getElementById('spc_Identificacion.ha_alquiladas').value;
}

function cargarSelectCultivos(id) {
    //var select = document.getElementById(value + "[" + id + "]");

    var lista = JSON.parse('@Html.Raw(Json.Serialize(ViewBag.listaCultivos))');

    var option = '';
    option += ' <option value="0">Seleccionar Tipo</option>';
    lista.forEach(function (element) {
        option += ' <option value="' + element.value + '">' + element.text + '</option>';
    });
    //$('#selectListaCultivos[' + id + ']').append(option);

    document.getElementById('selectListaCultivos[' + id + ']').innerHTML = option;

}
var idTiposDeCultivos = -1;
function agregarTiposDeCultivos() {
    var event_data = '';


    idTiposDeCultivos++;
    event_data += ' <tr>'
    event_data += '     <td>'
    event_data += '         <select id = "selectListaCultivos[' + idTiposDeCultivos + ']" onchange="cambioTipoCultivo(' + idTiposDeCultivos + ')">'
    event_data += '         </select >'
    event_data += '         <input class="e-input" name="listaCultivos[' + idTiposDeCultivos + '].id_cultivo" id="listaCultivos[' + idTiposDeCultivos + '].id_cultivo" type = "hidden" >'
    event_data += '         <input class="e-input" name="listaCultivos[' + idTiposDeCultivos + '].cultivo" id="listaCultivos[' + idTiposDeCultivos + '].cultivo" type="hidden">'
    event_data += '     </td>'
    event_data += '     <td>'
    event_data += '         <input class="e-input" name="listaCultivos[' + idTiposDeCultivos + '].ha_propias" type="number" >'
    event_data += '     </td>'
    event_data += '     <td>'
    event_data += '         <input class="e-input" name="listaCultivos[' + idTiposDeCultivos + '].ha_alquiladas" type="number" >'
    event_data += '     </td>'
    event_data += '     <td class="columnaAction">'
    event_data += '         <button class="btn btn-success"><i class="fa fa-check"></i></button>'
    event_data += '         <button class="btn btn-danger"><i class="fa fa-close"></i></button>'
    event_data += '     </td>'
    event_data += ' </tr>';

    $('#tableTiposDeCultivos').append(event_data);

    cargarSelectCultivos(idTiposDeCultivos);
}

function cambioTipoCultivo(id) {
    var select = document.getElementById('selectListaCultivos[' + id + ']');
    document.getElementById('listaCultivos[' + id + '].id_cultivo').value = select.options[select.selectedIndex].value;
    document.getElementById('listaCultivos[' + id + '].cultivo').value = select.options[select.selectedIndex].text;
}

// agregar maquinarias que posee el cliente
var idMaquinariasQuePosee = -1;
function agregarMaquinariasQuePosee() {
    var event_data = '';

    idMaquinariasQuePosee++;
    event_data += '<tr>'
    event_data += '    <td>'
    event_data += '        <input class="e-input" name="listaMaquinarias[' + idMaquinariasQuePosee + '].maquinaria" type="text">'
    event_data += '    </td>'
    event_data += '    <td>'
    event_data += '        <input class="e-input" name="listaMaquinarias[' + idMaquinariasQuePosee + '].marca" type="text">'
    event_data += '    </td>'
    event_data += '    <td>'
    event_data += '        <input class="e-input" name="listaMaquinarias[' + idMaquinariasQuePosee + '].detalle" type="text">'
    event_data += '    </td>'
    event_data += '    <td>'
    event_data += '        <input class="e-input" name="listaMaquinarias[' + idMaquinariasQuePosee + '].hp" type="text">'
    event_data += '    </td>'
    event_data += '    <td>'
    event_data += '        <input class="e-input" name="listaMaquinarias[' + idMaquinariasQuePosee + '].monto" type="number">'
    event_data += '    </td>'
    event_data += '    <td class="columnaAction">'
    event_data += '        <button class="btn btn-success"><i class="fa fa-check"></i></button>'
    event_data += '        <button class="btn btn-danger"><i class="fa fa-close"></i></button>'
    event_data += '    </td>'
    event_data += '</tr>';

    $('#tableMaquinariasQuePosee').append(event_data);
}

//agregar productos
var idProductos = -1;
function agregarProductos() {
    var event_data = '';

    idProductos++;
    event_data += '<tr>'
    event_data += '    <td>'
    event_data += '         <select id = "selectListaTiposTractores[' + idProductos + ']" onchange="cambioTiposTractores(' + idProductos + ')">'
    event_data += '         </select >'
    event_data += '         <input class="e-input" name="listaProductos[' + idProductos + '].id_tipo" id = "listaProductos[' + idProductos + '].id_tipo" type = "hidden" >'
    event_data += '         <input class="e-input" name="listaProductos[' + idProductos + '].tipo" id="listaProductos[' + idProductos + '].tipo" type="hidden">'
    event_data += '    </td>'
    event_data += '    <td>'
    event_data += '         <select id = "selectListaMarcas[' + idProductos + ']" onchange="cambioMarcas(' + idProductos + ')">'
    event_data += '         </select >'
    event_data += '         <input class="e-input" name= "listaProductos[' + idProductos + '].id_marca" id = "listaProductos[' + idProductos + '].id_marca" type = "hidden" >'
    event_data += '         <input class="e-input" name="listaProductos[' + idProductos + '].marca" id="listaProductos[' + idProductos + '].marca" type="hidden">'
    event_data += '    </td>'
    event_data += '    <td>'
    event_data += '        <input class="e-input" name="listaProductos[' + idProductos + '].modelo" id="listaProductos[' + idProductos + '].modelo" onchange="cambioModelo(' + idProductos + ')" type="text">'
    event_data += '    </td>'
    event_data += '    <td>'
    event_data += '        <input class="e-input" name="listaProductos[' + idProductos + '].monto" id="listaProductos[' + idProductos + '].monto" onchange="cambioMonto(' + idProductos + ')" type="number">'
    event_data += '    </td>'
    event_data += '    <td class="columnaAction">'
    event_data += '        <button class="btn btn-success"><i class="fa fa-check"></i></button>'
    event_data += '        <button class="btn btn-danger"><i class="fa fa-close"></i></button>'
    event_data += '    </td>'
    event_data += '</tr>';

    $('#tableProductos').append(event_data);

    cargarSelectTractores(idProductos);
    cargarSelectMarcas(idProductos);

    idMaquinarias[idProductos] = -1;

    cargarProductoMaquinaria(idProductos);
}

function cambioModelo(id) {
    document.getElementById('listaMaquinariaPlanDePago[' + id + '].maquinaria').value = document.getElementById('listaProductos[' + id + '].modelo').value;
}

function cambioMonto(id) {
    document.getElementById('listaMaquinariaPlanDePago[' + id + '].monto').value = document.getElementById('listaProductos[' + id + '].monto').value;
}
function cargarSelectTractores(id) {
    //var select = document.getElementById(value + "[" + id + "]");

    var lista = JSON.parse('@Html.Raw(Json.Serialize(ViewBag.listaTiposTractores))');

    var option = '';
    option += ' <option value="0">Seleccionar Tipo</option>';
    lista.forEach(function (element) {
        option += ' <option value="' + element.value + '">' + element.text + '</option>';
    });
    //$('#selectListaCultivos[' + id + ']').append(option);

    document.getElementById('selectListaTiposTractores[' + id + ']').innerHTML = option;

}

function cambioTiposTractores(id) {
    var select = document.getElementById('selectListaTiposTractores[' + id + ']');
    document.getElementById('listaProductos[' + id + '].id_tipo').value = select.options[select.selectedIndex].value;
    document.getElementById('listaProductos[' + id + '].tipo').value = select.options[select.selectedIndex].text;
}

function cargarSelectMarcas(id) {
    //var select = document.getElementById(value + "[" + id + "]");

    var lista = JSON.parse('@Html.Raw(Json.Serialize(ViewBag.listaMarcas))');

    var option = '';
    option += ' <option value="0">Seleccionar Marca</option>';
    lista.forEach(function (element) {
        option += ' <option value="' + element.value + '">' + element.text + '</option>';
    });
    //$('#selectListaCultivos[' + id + ']').append(option);

    document.getElementById('selectListaMarcas[' + id + ']').innerHTML = option;

}

function cambioMarcas(id) {
    var select = document.getElementById('selectListaMarcas[' + id + ']');
    document.getElementById('listaProductos[' + id + '].id_marca').value = select.options[select.selectedIndex].value;
    document.getElementById('listaProductos[' + id + '].marca').value = select.options[select.selectedIndex].text;
}

// para cargar productos maquinarias
function cargarProductoMaquinaria(id) {
    var event_data = '';

    event_data += '<div class="div_maquinaria">'
    event_data += '    <div class="row">'
    event_data += '        <div class="col-xs-12 col-sm-12 col-lg-4 col-md-4">'
    event_data += '            <h4> Maquinaria:</h4>'
    event_data += '           <div class="e-input-group">'
    event_data += '                <input class="e-input readonly" name="listaMaquinariaPlanDePago[' + id + '].maquinaria" id="listaMaquinariaPlanDePago[' + id + '].maquinaria" type="text" readonly>'
    event_data += '            </div>'
    event_data += '        </div>'
    event_data += '        <div class="col-xs-12 col-sm-12 col-lg-4 col-md-4">'
    event_data += '            <h4> Fecha Inicial:</h4>'
    event_data += '            <div class="e-input-group">'
    event_data += '                <input class="e-input" name="listaMaquinariaPlanDePago[' + id + '].fecha_inicial" type="date">'
    event_data += '            </div>'
    event_data += '       </div>'
    event_data += '        <div class="col-xs-12 col-sm-12 col-lg-4 col-md-4">'
    event_data += '            <h4> Monto:</h4>'
    event_data += '            <div class="e-input-group">'
    event_data += '                <input class="e-input readonly" name="listaMaquinariaPlanDePago[' + id + '].monto" id="listaMaquinariaPlanDePago[' + id + '].monto" type="number" readonly>'
    event_data += '            </div>'
    event_data += '        </div>'
    event_data += '    </div>'
    event_data += '    <div class="row">'
    event_data += '        <div class="col-xs-12 col-sm-12 col-lg-4 col-md-4">'
    event_data += '            <h4> Couta Inicial:</h4>'
    event_data += '            <div class="e-input-group">'
    event_data += '                <input class="e-input" name="listaMaquinariaPlanDePago[' + id + '].cuota_inicial" type="number">'
    event_data += '            </div>'
    event_data += '       </div>'
    event_data += '        <div class="col-xs-12 col-sm-12 col-lg-4 col-md-4">'
    event_data += '            <h4> Tasa de Interes Anual [%]:</h4>'
    event_data += '            <div class="e-input-group">'
    event_data += '                <input class="e-input" name="listaMaquinariaPlanDePago[' + id + '].interes" type="number">'
    event_data += '            </div>'
    event_data += '        </div>'
    event_data += '        <div class="col-xs-12 col-sm-12 col-lg-4 col-md-4">'
    event_data += '        </div>'
    event_data += '    </div>'

    event_data += '    <div class="cabecera_table" style="margin-top:1%">'
    event_data += '        <input type="button" class="btn btn-success buttonAgregar" onclick="agregarProductoMaquinarias(' + id + ')" value="Agregar" />'
    event_data += '    </div>'
    event_data += '   <table id="tableProductoMaquinarias' + id + '">'
    event_data += '        <tr>'
    event_data += '            <th>Nro</th>'
    event_data += '            <th>Fecha</th>'
    event_data += '            <th>Capital</th>'
    event_data += '            <th>Interes</th>'
    event_data += '            <th>Cuota</th>'
    event_data += '            <th></th>'
    event_data += '        </tr>'

    event_data += '    </table>'
    event_data += '</div>';

    $('#productosMaquinarias').append(event_data);
}

// cargar maquinarias items de productos
var idMaquinarias = [];
function agregarProductoMaquinarias(id) {
    var event_data = '';

    idMaquinarias[id] = idMaquinarias[id] + 1;
    event_data += '<tr>'
    event_data += '    <td>'
    event_data += '        ' + arrayNro[idMaquinarias[id]] + ''
    event_data += '    </td>'
    event_data += '    <td>'
    event_data += '        <input class="e-input" name="listaPlanDePagoMaquinaria[' + id + '][' + idMaquinarias[id] + '].fecha" type="date">'
    event_data += '    </td>'
    event_data += '    <td>'
    event_data += '        <input class="e-input" name="listaPlanDePagoMaquinaria[' + id + '][' + idMaquinarias[id] + '].capital" type="number">'
    event_data += '    </td>'
    event_data += '    <td>'
    event_data += '        <input class="e-input" name="listaPlanDePagoMaquinaria[' + id + '][' + idMaquinarias[id] + '].interes" type="number">'
    event_data += '   </td>'
    event_data += '    <td>'
    event_data += '        <input class="e-input" name="listaPlanDePagoMaquinaria[' + id + '][' + idMaquinarias[id] + '].cuota" type="number">'
    event_data += '    </td>'
    event_data += '    <td class="columnaAction">'
    event_data += '        <button class="btn btn-success"><i class="fa fa-check"></i></button>'
    event_data += '        <button class="btn btn-danger"><i class="fa fa-close"></i></button>'
    event_data += '   </td>'
    event_data += '</tr>'

    $('#tableProductoMaquinarias' + id + '').append(event_data);
}

var arrayNro = ["1ra", "2da", "3ra", "4ta", "5ta", "6ta", "7ma", "8va", "9na", "10ma", "11va", "12va", "13va", "14va", "15va", "16va", "17va", "18va", "19va", "20va", "21va", "22va", "23va", "24va", "25va"];

// ********************************************************************* Javascript para spc ****************************

