function check_file(ext) {
    array = ext.split(",");
    str = document.getElementById('file').value.toUpperCase();

    var len = 0;
    for (i in array) {
        var ext1 = '.' + array[i].toUpperCase();
        if (str.lastIndexOf(ext1) == -1) {
            ++len;
        }
    }
    if (len == array.length) {
        alert('Zły format pliku.\nDozwolone formaty: ' + array);
        document.getElementById('file').value = '';
    }
    
}