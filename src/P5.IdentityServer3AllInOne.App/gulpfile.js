/// <binding Clean='clean' />
"use strict";

var gulp = require("gulp"),
    rimraf = require("rimraf"),
    concat = require("gulp-concat"),
    cssmin = require("gulp-cssmin"),
    uglify = require("gulp-uglify"),
    gulpFilter = require('gulp-filter');

var paths = {
    webroot: "./wwwroot/"
};

paths.js = paths.webroot + "js/**/*.js";
paths.minJs = paths.webroot + "js/**/*.min.js";
paths.css = paths.webroot + "css/**/*.css";
paths.minCss = paths.webroot + "css/**/*.min.css";
paths.concatJsDest = paths.webroot + "js/site.min.js";
paths.concatCssDest = paths.webroot + "css/site.min.css";
paths.areas = "Areas";

gulp.task("clean:js", function (cb) {
    rimraf(paths.concatJsDest, cb);
});

gulp.task("clean:css", function (cb) {
    rimraf(paths.concatCssDest, cb);
});

gulp.task("clean", ["clean:js", "clean:css"]);

gulp.task("min:js", function () {
    return gulp.src([paths.js, "!" + paths.minJs], { base: "." })
        .pipe(concat(paths.concatJsDest))
        .pipe(uglify())
        .pipe(gulp.dest("."));
});

gulp.task("min:css", function () {
    return gulp.src([paths.css, "!" + paths.minCss])
        .pipe(concat(paths.concatCssDest))
        .pipe(cssmin())
        .pipe(gulp.dest("."));
});

gulp.task("clean:areas", function (cb) {
    rimraf(paths.areas, cb);
});



gulp.task('copy:P5.IdentityServer3.Admin:areas', function () {
    return gulp.src(['../P5.IdentityServer3.Admin/Areas/**', '!../P5.IdentityServer3.Admin/Areas/*/{Controllers,Controllers/**}', '!../P5.IdentityServer3.Admin/Areas/**/*.cs'])
        .pipe(gulp.dest('Areas/'));
});

gulp.task('copy:P5.IdentityServer3.Admin:assets', function () {
    return gulp.src(['../P5.IdentityServer3.Admin/assets/**'])
        .pipe(gulp.dest(paths.webroot + 'assets/'));
});





gulp.task('watch', [
       
        'copy:P5.IdentityServer3.Admin:areas',
        'copy:P5.IdentityServer3.Admin:assets'
],
    function () {      
        gulp.watch(['../P5.IdentityServer3.Admin/Areas/**'], ['copy:P5.IdentityServer3.Admin:areas']);
        gulp.watch(['../P5.IdentityServer3.Admin/assets/**'], ['copy:P5.IdentityServer3.Admin:assets']);
    });

gulp.task("min", ["min:js", "min:css"]);
