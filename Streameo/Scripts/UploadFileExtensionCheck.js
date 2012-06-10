function check_file(ext) {
    array = ext.split(",");

    for (i in array) {
        var ext1 = '.' + array[i].toUpperCase();
        str = document.getElementById('file').value.toUpperCase();
        if (str.lastIndexOf(ext1) == -1) {
            alert('Zły format pliku.\nDozwolone formaty: *' + ext1.toLowerCase());
            document.getElementById('file').value = '';
            break;
        }
    }
    
}