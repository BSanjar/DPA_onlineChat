 function searchFromTable () {
    const searchInput = document.getElementById("searchInput");
     const dataTable = document.getElementById("dataTable3");
    const rows = dataTable.querySelectorAll("tbody tr");

    searchInput.addEventListener("input", function () {
        const searchTerm = searchInput.value.toLowerCase();

        rows.forEach(function (row) {
            const cells = row.querySelectorAll("td");
            let rowMatch = false;

            cells.forEach(function (cell) {
                if (cell.textContent.toLowerCase().includes(searchTerm)) {
                    rowMatch = true;
                }
            });

            if (rowMatch) {
                row.style.display = "";
            } else {
                row.style.display = "none";
            }
        });
    });
}


function filterTable() {
    const filterRadios = document.querySelectorAll('input[name="filter"]');
    const dataTable = document.getElementById("dataTable");
    const rows = dataTable.querySelectorAll("tbody tr");

    // Обработчик изменения состояния радиокнопок
    filterRadios.forEach(function (radio) {
        radio.addEventListener("change", function () {
            const selectedValue = document.querySelector('input[name="filter"]:checked').value;

            rows.forEach(function (row) {
                const cellValue = row.querySelector("td:nth-child(1)").textContent; 

                if (selectedValue === "all" || cellValue === selectedValue) {
                    //console.log(cellValue);
                    row.style.display = "";
                } else {
                    row.style.display = "none";
                }
            });
        });
    });
}


//document.addEventListener("DOMContentLoaded", function () {
//    const table = document.getElementById("dataTable");
//    const tbody = table.querySelector("tbody");
//    const rows = Array.from(tbody.querySelectorAll("tr"));
//    const sortingState = {}; // Объект для отслеживания состояния сортировки по столбцам

//    // Функция для преобразования даты из строки в объект Date
//    function parseDate(dateString) {
//        const parts = dateString.split("-");
//        return new Date(`${parts[2]}-${parts[1]}-${parts[0]}`);
//    }

//    // Функция для сортировки строк по указанному столбцу и типу данных
//    function sortTable(column, dataType) {
//        const isAscending = sortingState[column] === "asc";

//        rows.sort((a, b) => {
//            const cellA = a.querySelector(`td:nth-child(${column})`).textContent;
//            const cellB = b.querySelector(`td:nth-child(${column})`).textContent;

//            if (dataType === "number") {
//                return isAscending ? parseFloat(cellA) - parseFloat(cellB) : parseFloat(cellB) - parseFloat(cellA);
//            } else if (dataType === "date") {
//                const dateA = parseDate(cellA);
//                const dateB = parseDate(cellB);
//                return isAscending ? dateA - dateB : dateB - dateA;
//            } else {
//                return isAscending ? cellA.localeCompare(cellB) : cellB.localeCompare(cellA);
//            }
//        });

//        sortingState[column] = isAscending ? "desc" : "asc";

//        tbody.innerHTML = "";
//        rows.forEach(row => {
//            tbody.appendChild(row);
//        });
//    }

    // Обработчик клика по заголовкам столбцов
//    table.addEventListener("click", function (e) {
//        if (e.target.tagName === "TH") {
//            const column = e.target.getAttribute("data-column");
//            const dataType = e.target.getAttribute("data-type");
//            if (column) {
//                sortTable(parseInt(column, 10), dataType);
//            }
//        }
//    });
//});


// Функция для изменения культуры и обновления страницы
//function changeLanguage(culture) {
    //$.ajax({
    //    type: 'POST',
    //    url: '/Global/SetLanguage',
    //    data: {
    //        culture: culture
    //    },
    //    success: function (data) {
    //        ////console.log(selectedLn);
    //        location.reload();
    //        document.cookie = `selectedCulture=${culture}; expires=Fri, 31 Dec 9999 23:59:59 GMT`;
    //         ////console.log(selectedLn);
    //        //const selectedLn = document.getElementById(data);
    //        //$(selectedLn).addClass('nav-active');
    //    },
    //    error: function (xhr, str) {
    //        //console.log("Не удалось переключить язык")
    //    }
    //});
//}

function loadTranslationsAndStore() {
    $.ajax({
        url: '/Global/GetTranslations', // Замените на реальный URL контроллера и действия
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            // Сохраняем полученные данные в LocalStorage
            localStorage.setItem('translations', JSON.stringify(data));
        },
        error: function (error) {
            console.error('Ошибка при загрузке данных:', error);
        }
    });
}

// Вызываем функцию при загрузке страницы
$(document).ready(function () {
    const currentLanguage = localStorage.getItem('currentLanguage');
    if (currentLanguage !== null) {
        changeLanguage(currentLanguage);
        loadTranslationsAndStore();
    }
    else {
        localStorage.setItem('currentLanguage', 'ru');
        loadTranslationsAndStore();
    }
});

function changeLanguage(selectedLanguage) {
    // Получаем текущий язык из LocalStorage
    let currentLanguage = localStorage.getItem('currentLanguage');
    //console.log(selectedLanguage);
    //console.log(currentLanguage);
    if (selectedLanguage === currentLanguage) {
        currentLanguage = 'ru';
    }

    // Получаем сохраненные данные из LocalStorage
    const translations = JSON.parse(localStorage.getItem('translations'));

    //console.log($('.hasTrans'));

    // Перебираем все элементы с классом 'hasTrans'
    $('.hasTrans').each(function () {
        const originalText = $(this).text().trim(); // Получаем оригинальный текст элемента
        //console.log(originalText);
        const translationObject = translations.find(obj => obj[currentLanguage] === originalText);
        //console.log(translationObject);
        let translatedText = null;
        if (translationObject) {
            translatedText = translationObject[selectedLanguage];
            //console.log(translatedText);
            if (translatedText !== null && translatedText !== "") {
                $(this).text(translatedText);
                
            }
        }        
    });

    //перебор placeholder
    $('.hasTransPh').each(function () {
        const placeholderText = $(this).attr('placeholder'); // Получаем оригинальный текст атрибута placeholder

        const translationObject = translations.find(obj => obj[currentLanguage] === placeholderText);
        let translatedText = null;

        if (translationObject) {
            translatedText = translationObject[selectedLanguage];
            if (translatedText !== null && translatedText !== undefined && translatedText.trim() !== "") {
                $(this).attr('placeholder', translatedText); // Устанавливаем переведенный текст в атрибут placeholder
            }
        }
    });

      //сбрасываю выбранный язык
        $('.lnEl').removeClass('nav-active');
        // Добавляем класс 'nav-active' к выбранному элементу
    $(`#${selectedLanguage}Ln`).addClass('nav-active');  
        localStorage.setItem('currentLanguage', selectedLanguage);
}


   


//$(function () {

    

    //$('.lnEl').removeClass('nav-active');
    //const selectedCulture = getCookie('selectedCulture'); // Получаем значение куки

    //if (selectedCulture) {
    //    // Добавляем класс 'nav-active' к выбранному элементу
    //    $(`#${selectedCulture}Ln`).addClass('nav-active');
    //}
//});

function getCookie(name) {
    const cookieValue = document.cookie.match('(^|;)\\s*' + name + '\\s*=\\s*([^;]+)');
    return cookieValue ? cookieValue.pop() : null;
}

//function GetCreditAccountInfo() {
//    var acc = document.getElementById("accountCreditNumber").value;
//    if (acc.length == 16) {
//        document.getElementById("creditAcc").style.display = "block";
//        document.getElementById("creditAcc").innerHTML = "<label style=\"color:red\">Поиск информации по счету...</label>";

//        $.ajax({
//            type: 'POST',
//            url: '@Url.Action("GetCreditAccountInfo", "Operator")',
//            data: {
//                acc: acc
//            },
//            success: function (data) {
//                document.getElementById("creditAcc").style.display = "block";
//                document.getElementById("creditAcc").innerHTML = data;
//            },
//            error: function (xhr, str) {
//                document.getElementById("creditAcc").style.display = "block";
//                document.getElementById("creditAcc").innerHTML = "Возникла ошибка, попробуйте еще раз!";
//            }
//        });
//    }
//}

function filterTable2() {
    const form = document.getElementById('filter');
    form.submit();
}



/* автоматичесское скрытие\расскрытие контейнера*/
function hideBlock(containerId) {

    if (containerId === "all") {
        var allBlock = document.getElementById("handlerDiv");
        var allElements = allBlock.querySelectorAll('input, textarea');
        for (var i = 0; i < allElements.length; i++) {
            if (allElements[i].value.trim() === '') {
                alert("Заполните все разделы обработчика!")
                return false;
            }
        }

        var allElements2 = allBlock.querySelectorAll('.hideBlock');
        for (var i = 0; i < allElements2.length; i++) {
            allElements2[i].style.display = 'none';
        }

        var allHandlerBlocks = allBlock.querySelectorAll('.handlerBlock');
        for (var i = 0; i < allHandlerBlocks.length; i++) {
            
            $(allHandlerBlocks[i]).addClass('expanded');
        }

        return true;
    }

    var handlerBlock = document.getElementById("handlerBlock_" + containerId);
    var elements = handlerBlock.querySelectorAll('input, textarea');

    for (var i = 0; i < elements.length; i++) {
        if (elements[i].value.trim() === '') {
            alert("Заполните все разделы обработчика!")
            return false;
        }
    }
    var containerHide = document.getElementById("hideBlock_" + containerId);

    if (containerHide.style.display === 'none') {
        containerHide.style.display = 'block';
    } else {
        containerHide.style.display = 'none';
    }
    handlerBlock.classList.toggle('expanded');
    return true;
   
}


/* автоматичесское расширение textarea*/
document.addEventListener("input", function (event) {
    if (event.target.classList.contains("sizingText")) {
        var textarea = event.target;
        textarea.style.height = "auto";
        textarea.style.height = (textarea.scrollHeight) + "px";
    }
});


