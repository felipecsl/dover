@echo off
pushd %1
call java -jar C:\Data\projects\closure-library\closure\bin\compiler.jar --compilation_level=WHITESPACE_ONLY ^
--js=%1ckfinder\ckfinder.js ^
--js=%1Scripts\dover.js ^
--js=%1Scripts\breadcrumb.js ^
--js=%1Scripts\libs\jquery.menuButton.js ^
--js=%1Scripts\libs\jquery.ajaxfileupload.js ^
--js=%1Scripts\libs\jquery.lazyload.mini.js ^
--js=%1Scripts\libs\jquery.tmpl.min.js ^
--js=%1Scripts\libs\jquery.cookie.js ^
--js=%1Scripts\libs\jquery.maskMoney.js ^
--js=%1Scripts\libs\jquery.Jcrop.min.js ^
--js=%1Scripts\libs\JSLINQ.js ^
--js=%1Scripts\libs\json2.min.js ^
--js=%1Scripts\libs\jquery.tablesorter.min.js ^
--js=%1Scripts\libs\browserupdate.js ^
--js=%1Scripts\libs\ui.datepicker-pt-BR.js ^
--js=%1Scripts\libs\jquery.tablednd_0_5.js ^
--js=%1Scripts\libs\jquery.flot.min.js ^
--js=%1Scripts\libs\swfupload\swfupload.min.js ^
--js=%1Scripts\libs\swfupload\swfupload.queue.min.js ^
--js=%1Scripts\fieldtemplates\imagelist.js ^
--js=%1Scripts\utilities\utils.js ^
--js=%1Scripts\utilities\date.format.min.js ^
--js=%1Scripts\utilities\mediamanager\mediamanager.js ^
--js=%1Scripts\utilities\mediamanager\mediamanager.swfupload.js ^
--js=%1Scripts\utilities\mediamanager\mediamanager.file.js ^
--js=%1Scripts\utilities\mediamanager\mediamanager.filelist.js ^
--js=%1Scripts\admin\moduleeditor\moduledetails.js ^
--js=%1Scripts\admin\moduleeditor\fieldeditor\fieldmanager.js ^
--js=%1Scripts\admin\moduleeditor\fieldeditor\modulereferencedialog.js ^
--js=%1Scripts\admin\moduleeditor\fieldeditor\validvaluesdialog.js ^
--js_output_file=Scripts\dover-compiled.js
popd