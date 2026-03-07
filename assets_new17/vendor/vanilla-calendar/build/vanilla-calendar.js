/*! name: @uvarov.frontend/vanilla-calendar | url: https://github.com/uvarov-frontend/vanilla-calendar */
(function webpackUniversalModuleDefinition(root, factory) {
	if(typeof exports === 'object' && typeof module === 'object')
		module.exports = factory();
	else if(typeof define === 'function' && define.amd)
		define([], factory);
	else {
		var a = factory();
		for(var i in a) (typeof exports === 'object' ? exports : root)[i] = a[i];
	}
})(self, () => {
return /******/ (() => { // webpackBootstrap
/******/ 	"use strict";
/******/ 	var __webpack_modules__ = ({

/***/ "./src/scripts/methods/changeMonth.ts":
/*!********************************************!*\
  !*** ./src/scripts/methods/changeMonth.ts ***!
  \********************************************/
/***/ ((__unused_webpack_module, __webpack_exports__, __webpack_require__) => {

__webpack_require__.r(__webpack_exports__);
/* harmony export */ __webpack_require__.d(__webpack_exports__, {
/* harmony export */   "default": () => (__WEBPACK_DEFAULT_EXPORT__)
/* harmony export */ });
/* harmony import */ var _controlArrows__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ./controlArrows */ "./src/scripts/methods/controlArrows.ts");
/* harmony import */ var _createDays__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ./createDays */ "./src/scripts/methods/createDays.ts");
/* harmony import */ var _createHeader__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! ./createHeader */ "./src/scripts/methods/createHeader.ts");




const changeMonth = (self, route) => {
  if (self.selectedMonth === void 0 || self.selectedYear === void 0)
    return;
  const lastMonth = self.locale.months.length - 1;
  switch (route) {
    case "prev":
      if (self.selectedMonth !== 0) {
        self.selectedMonth -= 1;
      } else if (self.settings.selection.year) {
        self.selectedYear -= 1;
        self.selectedMonth = lastMonth;
      }
      break;
    case "next":
      if (self.selectedMonth !== lastMonth) {
        self.selectedMonth += 1;
      } else if (self.settings.selection.year) {
        self.selectedYear += 1;
        self.selectedMonth = 0;
      }
      break;
  }
  self.settings.selected.month = self.selectedMonth;
  self.settings.selected.year = self.selectedYear;
  (0,_createHeader__WEBPACK_IMPORTED_MODULE_2__["default"])(self);
  (0,_controlArrows__WEBPACK_IMPORTED_MODULE_0__["default"])(self);
  (0,_createDays__WEBPACK_IMPORTED_MODULE_1__["default"])(self);
};
/* harmony default export */ const __WEBPACK_DEFAULT_EXPORT__ = (changeMonth);


/***/ }),

/***/ "./src/scripts/methods/clickCalendar.ts":
/*!**********************************************!*\
  !*** ./src/scripts/methods/clickCalendar.ts ***!
  \**********************************************/
/***/ ((__unused_webpack_module, __webpack_exports__, __webpack_require__) => {

__webpack_require__.r(__webpack_exports__);
/* harmony export */ __webpack_require__.d(__webpack_exports__, {
/* harmony export */   "default": () => (__WEBPACK_DEFAULT_EXPORT__)
/* harmony export */ });
/* harmony import */ var _changeMonth__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ./changeMonth */ "./src/scripts/methods/changeMonth.ts");
/* harmony import */ var _createDays__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ./createDays */ "./src/scripts/methods/createDays.ts");
/* harmony import */ var _createMonths__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! ./createMonths */ "./src/scripts/methods/createMonths.ts");
/* harmony import */ var _createYears__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! ./createYears */ "./src/scripts/methods/createYears.ts");
/* harmony import */ var _generateDate__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! ./generateDate */ "./src/scripts/methods/generateDate.ts");
/* harmony import */ var _updateCalendar__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! ./updateCalendar */ "./src/scripts/methods/updateCalendar.ts");







const clickCalendar = (self) => {
  self.HTMLElement.addEventListener("click", (e) => {
    const element = e.target;
    const arrowEl = element.closest(".vanilla-calendar-arrow");
    const arrowPrevEl = element.closest(".vanilla-calendar-arrow_prev");
    const arrowNextEl = element.closest(".vanilla-calendar-arrow_next");
    const dayBtnEl = element.closest(".vanilla-calendar-day__btn");
    const dayBtnPrevEl = element.closest(".vanilla-calendar-day__btn_prev");
    const dayBtnNextEl = element.closest(".vanilla-calendar-day__btn_next");
    const yearHeaderEl = element.closest(".vanilla-calendar-year");
    const yearItemEl = element.closest(".vanilla-calendar-years__year");
    const monthHeaderEl = element.closest(".vanilla-calendar-month");
    const monthItemEl = element.closest(".vanilla-calendar-months__month");
    const clickArrowMonth = () => {
      if (arrowEl && self.currentType !== "year" && self.currentType !== "month") {
        (0,_changeMonth__WEBPACK_IMPORTED_MODULE_0__["default"])(self, element.dataset.calendarArrow);
      }
    };
    const clickDaySingle = () => {
      if (!self.selectedDates || !dayBtnEl || !dayBtnEl.dataset.calendarDay)
        return;
      if (dayBtnEl.classList.contains("vanilla-calendar-day__btn_selected")) {
        self.selectedDates.splice(self.selectedDates.indexOf(dayBtnEl.dataset.calendarDay), 1);
      } else {
        self.selectedDates = [];
        self.selectedDates.push(dayBtnEl.dataset.calendarDay);
      }
    };
    const clickDayMultiple = () => {
      if (!self.selectedDates || !dayBtnEl || !dayBtnEl.dataset.calendarDay)
        return;
      if (dayBtnEl.classList.contains("vanilla-calendar-day__btn_selected")) {
        self.selectedDates.splice(self.selectedDates.indexOf(dayBtnEl.dataset.calendarDay), 1);
      } else {
        self.selectedDates.push(dayBtnEl.dataset.calendarDay);
      }
    };
    const clickDayMultipleRanged = () => {
      if (!self.selectedDates || !dayBtnEl || !dayBtnEl.dataset.calendarDay)
        return;
      if (self.selectedDates.length > 1)
        self.selectedDates = [];
      self.selectedDates.push(dayBtnEl.dataset.calendarDay);
      if (!self.selectedDates[1])
        return;
      const startDate = new Date(Date.UTC(
        new Date(self.selectedDates[0]).getUTCFullYear(),
        new Date(self.selectedDates[0]).getUTCMonth(),
        new Date(self.selectedDates[0]).getUTCDate()
      ));
      const endDate = new Date(Date.UTC(
        new Date(self.selectedDates[1]).getUTCFullYear(),
        new Date(self.selectedDates[1]).getUTCMonth(),
        new Date(self.selectedDates[1]).getUTCDate()
      ));
      const addSelectedDate = (day) => {
        if (!self.selectedDates)
          return;
        const date = (0,_generateDate__WEBPACK_IMPORTED_MODULE_4__["default"])(day);
        if (self.settings.range.disabled && self.settings.range.disabled.includes(date))
          return;
        self.selectedDates.push(date);
      };
      self.selectedDates = [];
      if (endDate > startDate) {
        for (let i = startDate; i <= endDate; i.setUTCDate(i.getUTCDate() + 1)) {
          addSelectedDate(i);
        }
      } else {
        for (let i = startDate; i >= endDate; i.setUTCDate(i.getUTCDate() - 1)) {
          addSelectedDate(i);
        }
      }
    };
    const clickDay = () => {
      if (["single", "multiple", "multiple-ranged"].includes(self.settings.selection.day) && dayBtnEl) {
        switch (self.settings.selection.day) {
          case "single":
            clickDaySingle();
            break;
          case "multiple":
            clickDayMultiple();
            break;
          case "multiple-ranged":
            clickDayMultipleRanged();
            break;
        }
        if (self.actions.clickDay)
          self.actions.clickDay(e, self.selectedDates);
        self.settings.selected.dates = self.selectedDates;
        if (dayBtnPrevEl) {
          (0,_changeMonth__WEBPACK_IMPORTED_MODULE_0__["default"])(self, "prev");
        } else if (dayBtnNextEl) {
          (0,_changeMonth__WEBPACK_IMPORTED_MODULE_0__["default"])(self, "next");
        } else {
          (0,_createDays__WEBPACK_IMPORTED_MODULE_1__["default"])(self);
        }
      }
    };
    const clickYear = () => {
      if (!self.settings.selection.year)
        return;
      if (arrowEl && self.currentType === "year") {
        if (self.viewYear === void 0)
          return;
        if (arrowNextEl) {
          self.viewYear += 15;
        } else if (arrowPrevEl) {
          self.viewYear -= 15;
        }
        (0,_createYears__WEBPACK_IMPORTED_MODULE_3__["default"])(self);
      } else if (self.currentType !== "year" && yearHeaderEl) {
        (0,_createYears__WEBPACK_IMPORTED_MODULE_3__["default"])(self);
      } else if (self.currentType === "year" && yearHeaderEl) {
        self.currentType = self.type;
        (0,_updateCalendar__WEBPACK_IMPORTED_MODULE_5__["default"])(self);
      } else if (yearItemEl) {
        if (self.selectedMonth === void 0 || !self.dateMin || !self.dateMax)
          return;
        self.selectedYear = Number(yearItemEl.dataset.calendarYear);
        self.currentType = self.type;
        if (self.selectedMonth < self.dateMin.getUTCMonth() && self.selectedYear === self.dateMin.getUTCFullYear()) {
          self.settings.selected.month = self.dateMin.getUTCMonth();
        }
        if (self.selectedMonth > self.dateMax.getUTCMonth() && self.selectedYear === self.dateMax.getUTCFullYear()) {
          self.settings.selected.month = self.dateMax.getUTCMonth();
        }
        if (self.actions.clickYear)
          self.actions.clickYear(e, self.selectedYear);
        self.settings.selected.year = self.selectedYear;
        (0,_updateCalendar__WEBPACK_IMPORTED_MODULE_5__["default"])(self);
      }
    };
    const clickMonth = () => {
      if (!self.settings.selection.month)
        return;
      if (self.currentType !== "month" && monthHeaderEl) {
        (0,_createMonths__WEBPACK_IMPORTED_MODULE_2__["default"])(self);
      } else if (self.currentType === "month" && monthHeaderEl) {
        self.currentType = self.type;
        (0,_updateCalendar__WEBPACK_IMPORTED_MODULE_5__["default"])(self);
      } else if (monthItemEl) {
        self.selectedMonth = Number(monthItemEl.dataset.calendarMonth);
        self.currentType = self.type;
        if (self.actions.clickMonth)
          self.actions.clickMonth(e, self.selectedMonth);
        self.settings.selected.month = self.selectedMonth;
        (0,_updateCalendar__WEBPACK_IMPORTED_MODULE_5__["default"])(self);
      }
    };
    clickArrowMonth();
    clickDay();
    clickYear();
    clickMonth();
  });
};
/* harmony default export */ const __WEBPACK_DEFAULT_EXPORT__ = (clickCalendar);


/***/ }),

/***/ "./src/scripts/methods/controlArrows.ts":
/*!**********************************************!*\
  !*** ./src/scripts/methods/controlArrows.ts ***!
  \**********************************************/
/***/ ((__unused_webpack_module, __webpack_exports__, __webpack_require__) => {

__webpack_require__.r(__webpack_exports__);
/* harmony export */ __webpack_require__.d(__webpack_exports__, {
/* harmony export */   "default": () => (__WEBPACK_DEFAULT_EXPORT__)
/* harmony export */ });

const controlArrows = (self) => {
  if (!["default", "year"].includes(self.currentType))
    return;
  const arrowPrev = self.HTMLElement.querySelector(".vanilla-calendar-arrow_prev");
  const arrowNext = self.HTMLElement.querySelector(".vanilla-calendar-arrow_next");
  if (arrowPrev instanceof HTMLElement && arrowNext instanceof HTMLElement) {
    const defaultControl = () => {
      if (!self.dateMin || !self.dateMax || self.currentType !== "default")
        return;
      const isSelectedMinMount = self.selectedMonth === self.dateMin.getUTCMonth();
      const isSelectedMaxMount = self.selectedMonth === self.dateMax.getUTCMonth();
      const isSelectedMinYear = !self.settings.selection.year ? true : self.selectedYear === self.dateMin.getUTCFullYear();
      const isSelectedMaxYear = !self.settings.selection.year ? true : self.selectedYear === self.dateMax.getUTCFullYear();
      if (isSelectedMinMount && isSelectedMinYear || !self.settings.selection.month) {
        arrowPrev.style.visibility = "hidden";
      } else {
        arrowPrev.style.visibility = "";
      }
      if (isSelectedMaxMount && isSelectedMaxYear || !self.settings.selection.month) {
        arrowNext.style.visibility = "hidden";
      } else {
        arrowNext.style.visibility = "";
      }
    };
    const yearControl = () => {
      if (!self.dateMin || !self.dateMax || self.currentType !== "year" || self.viewYear === void 0)
        return;
      if (self.dateMin.getUTCFullYear() && self.viewYear - 7 <= self.dateMin.getUTCFullYear()) {
        arrowPrev.style.visibility = "hidden";
      } else {
        arrowPrev.style.visibility = "";
      }
      if (self.dateMax.getUTCFullYear() && self.viewYear + 7 >= self.dateMax.getUTCFullYear()) {
        arrowNext.style.visibility = "hidden";
      } else {
        arrowNext.style.visibility = "";
      }
    };
    defaultControl();
    yearControl();
  }
};
/* harmony default export */ const __WEBPACK_DEFAULT_EXPORT__ = (controlArrows);


/***/ }),

/***/ "./src/scripts/methods/controlTime.ts":
/*!********************************************!*\
  !*** ./src/scripts/methods/controlTime.ts ***!
  \********************************************/
/***/ ((__unused_webpack_module, __webpack_exports__, __webpack_require__) => {

__webpack_require__.r(__webpack_exports__);
/* harmony export */ __webpack_require__.d(__webpack_exports__, {
/* harmony export */   "default": () => (__WEBPACK_DEFAULT_EXPORT__)
/* harmony export */ });
/* harmony import */ var _transformTime12__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ./transformTime12 */ "./src/scripts/methods/transformTime12.ts");
/* harmony import */ var _transformTime24__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ./transformTime24 */ "./src/scripts/methods/transformTime24.ts");



const controlTime = (self, keepingTime) => {
  const rangeHours = self.HTMLElement.querySelector('.vanilla-calendar-time__range input[name="hours"]');
  const rangeMinutes = self.HTMLElement.querySelector('.vanilla-calendar-time__range input[name="minutes"]');
  const inputHours = self.HTMLElement.querySelector('.vanilla-calendar-time__hours input[name="hours"]');
  const inputMinutes = self.HTMLElement.querySelector('.vanilla-calendar-time__minutes input[name="minutes"]');
  const btnKeepingTime = self.HTMLElement.querySelector(".vanilla-calendar-time__keeping");
  const mouseoverRange = (range, input) => {
    range.addEventListener("mouseover", () => input.classList.add("is-focus"));
  };
  const mouseoutRange = (range, input) => {
    range.addEventListener("mouseout", () => input.classList.remove("is-focus"));
  };
  const setTime = (e, value, type) => {
    if (type === "hours") {
      self.selectedHours = `${value}`;
    } else if (type === "minutes") {
      self.selectedMinutes = `${value}`;
    }
    self.selectedTime = `${self.selectedHours}:${self.selectedMinutes}${self.selectedKeeping ? ` ${self.selectedKeeping}` : ""}`;
    self.settings.selected.time = self.selectedTime;
    if (self.actions.changeTime) {
      self.actions.changeTime(e, self.selectedTime, self.selectedHours, self.selectedMinutes, self.selectedKeeping);
    }
  };
  const changeRange = (range, input, type, max) => {
    range.addEventListener("input", (e) => {
      let value = Number(e.target.value);
      value = value < 10 ? `0${value}` : `${value}`;
      if (type === "hours" && max === 12) {
        if (Number(e.target.value) < max && Number(e.target.value) > 0) {
          input.value = value;
          self.selectedKeeping = "AM";
          btnKeepingTime.innerText = self.selectedKeeping;
          setTime(e, value, type);
        } else {
          if (Number(e.target.value) === 0) {
            self.selectedKeeping = "AM";
            btnKeepingTime.innerText = "AM";
          } else {
            self.selectedKeeping = "PM";
            btnKeepingTime.innerText = "PM";
          }
          input.value = (0,_transformTime12__WEBPACK_IMPORTED_MODULE_0__["default"])(e.target.value);
          setTime(e, (0,_transformTime12__WEBPACK_IMPORTED_MODULE_0__["default"])(e.target.value), type);
        }
      } else {
        input.value = value;
        setTime(e, value, type);
      }
    });
  };
  const changeInput = (range, input, type, max) => {
    input.addEventListener("change", (e) => {
      const changeInputEl = e.target;
      let value = Number(changeInputEl.value);
      value = value < 10 ? `0${value}` : `${value}`;
      if (type === "hours" && max === 12) {
        if (changeInputEl.value && Number(changeInputEl.value) <= max && Number(changeInputEl.value) > 0) {
          changeInputEl.value = value;
          range.value = (0,_transformTime24__WEBPACK_IMPORTED_MODULE_1__["default"])(value, self.selectedKeeping);
          setTime(e, value, type);
        } else if (changeInputEl.value && Number(changeInputEl.value) < 24 && (Number(changeInputEl.value) > max || Number(changeInputEl.value) === 0)) {
          if (Number(changeInputEl.value) === 0) {
            self.selectedKeeping = "AM";
            btnKeepingTime.innerText = "AM";
          } else {
            self.selectedKeeping = "PM";
            btnKeepingTime.innerText = "PM";
          }
          changeInputEl.value = (0,_transformTime12__WEBPACK_IMPORTED_MODULE_0__["default"])(changeInputEl.value);
          range.value = value;
          setTime(e, (0,_transformTime12__WEBPACK_IMPORTED_MODULE_0__["default"])(changeInputEl.value), type);
        } else {
          changeInputEl.value = self.selectedHours;
        }
      } else if (changeInputEl.value && Number(changeInputEl.value) <= max && Number(changeInputEl.value) >= 0) {
        changeInputEl.value = value;
        range.value = value;
        setTime(e, value, type);
      } else if (type === "hours") {
        changeInputEl.value = self.selectedHours;
      } else if (type === "minutes") {
        changeInputEl.value = self.selectedMinutes;
      }
    });
  };
  mouseoverRange(rangeHours, inputHours);
  mouseoverRange(rangeMinutes, inputMinutes);
  mouseoutRange(rangeHours, inputHours);
  mouseoutRange(rangeMinutes, inputMinutes);
  changeRange(rangeHours, inputHours, "hours", keepingTime === 24 ? 23 : 12);
  changeRange(rangeMinutes, inputMinutes, "minutes", 0);
  changeInput(rangeHours, inputHours, "hours", keepingTime === 24 ? 23 : 12);
  changeInput(rangeMinutes, inputMinutes, "minutes", 59);
  if (!btnKeepingTime)
    return;
  btnKeepingTime.addEventListener("click", (e) => {
    if (btnKeepingTime.innerText.includes("AM")) {
      self.selectedKeeping = "PM";
    } else {
      self.selectedKeeping = "AM";
    }
    rangeHours.value = (0,_transformTime24__WEBPACK_IMPORTED_MODULE_1__["default"])(self.selectedHours, self.selectedKeeping);
    setTime(e, self.selectedHours, "hours");
    btnKeepingTime.innerText = self.selectedKeeping;
  });
};
/* harmony default export */ const __WEBPACK_DEFAULT_EXPORT__ = (controlTime);


/***/ }),

/***/ "./src/scripts/methods/createDOM.ts":
/*!******************************************!*\
  !*** ./src/scripts/methods/createDOM.ts ***!
  \******************************************/
/***/ ((__unused_webpack_module, __webpack_exports__, __webpack_require__) => {

__webpack_require__.r(__webpack_exports__);
/* harmony export */ __webpack_require__.d(__webpack_exports__, {
/* harmony export */   "default": () => (__WEBPACK_DEFAULT_EXPORT__)
/* harmony export */ });

const createDOM = (self) => {
  const calendarElement = self.HTMLElement;
  if (self.currentType === "default") {
    calendarElement.classList.add("vanilla-calendar_default");
    calendarElement.classList.remove("vanilla-calendar_month");
    calendarElement.classList.remove("vanilla-calendar_year");
    calendarElement.innerHTML = `
		<div class="vanilla-calendar-header">
			<button type="button"
				class="vanilla-calendar-arrow vanilla-calendar-arrow_prev"
				data-calendar-arrow="prev"
				title="Prev">
			</button>
			<div class="vanilla-calendar-header__content"></div>
			<button type="button"
				class="vanilla-calendar-arrow vanilla-calendar-arrow_next"
				data-calendar-arrow="next"
				title="Next">
			</button>
		</div>
		${self.settings.visibility.weekNumbers ? `
		<div class="vanilla-calendar-column">
			<b class="vanilla-calendar-column__title">#</b>
			<div class="vanilla-calendar-column__content vanilla-calendar-week-numbers"></div>
		</div>
		` : ""}
		<div class="vanilla-calendar-content">
			<div class="vanilla-calendar-week"></div>
			<div class="vanilla-calendar-days"></div>
		</div>
		${self.settings.selection.time ? `
		<div class="vanilla-calendar-time"></div>
		` : ""}
	`;
  } else if (self.currentType === "month") {
    calendarElement.classList.remove("vanilla-calendar_default");
    calendarElement.classList.add("vanilla-calendar_month");
    calendarElement.classList.remove("vanilla-calendar_year");
    calendarElement.innerHTML = `
		<div class="vanilla-calendar-header">
			<div class="vanilla-calendar-header__content"></div>
		</div>
		<div class="vanilla-calendar-content">
			<div class="vanilla-calendar-months"></div>
		</div>`;
  } else if (self.currentType === "year") {
    calendarElement.classList.remove("vanilla-calendar_default");
    calendarElement.classList.remove("vanilla-calendar_month");
    calendarElement.classList.add("vanilla-calendar_year");
    calendarElement.innerHTML = `
		<div class="vanilla-calendar-header">
			<button type="button"
				class="vanilla-calendar-arrow vanilla-calendar-arrow_prev"
				title="prev">
			</button>
			<div class="vanilla-calendar-header__content"></div>
			<button type="button"
				class="vanilla-calendar-arrow vanilla-calendar-arrow_next"
				title="next">
			</button>
		</div>
		<div class="vanilla-calendar-content">
			<div class="vanilla-calendar-years"></div>
		</div>`;
  }
};
/* harmony default export */ const __WEBPACK_DEFAULT_EXPORT__ = (createDOM);


/***/ }),

/***/ "./src/scripts/methods/createDays.ts":
/*!*******************************************!*\
  !*** ./src/scripts/methods/createDays.ts ***!
  \*******************************************/
/***/ ((__unused_webpack_module, __webpack_exports__, __webpack_require__) => {

__webpack_require__.r(__webpack_exports__);
/* harmony export */ __webpack_require__.d(__webpack_exports__, {
/* harmony export */   "default": () => (__WEBPACK_DEFAULT_EXPORT__)
/* harmony export */ });
/* harmony import */ var _createPopup__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ./createPopup */ "./src/scripts/methods/createPopup.ts");
/* harmony import */ var _createWeekNumbers__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ./createWeekNumbers */ "./src/scripts/methods/createWeekNumbers.ts");
/* harmony import */ var _generateDate__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! ./generateDate */ "./src/scripts/methods/generateDate.ts");




const createDays = (self) => {
  if (self.selectedMonth === void 0 || self.selectedYear === void 0)
    return;
  const firstDay = new Date(Date.UTC(self.selectedYear, self.selectedMonth, 1));
  const daysSelectedMonth = new Date(Date.UTC(self.selectedYear, self.selectedMonth + 1, 0)).getUTCDate();
  let firstDayWeek = Number(firstDay.getUTCDay());
  if (self.settings.iso8601)
    firstDayWeek = Number((firstDay.getUTCDay() !== 0 ? firstDay.getUTCDay() : 7) - 1);
  const daysEl = self.HTMLElement.querySelector(".vanilla-calendar-days");
  if (!daysEl)
    return;
  const templateDayEl = document.createElement("div");
  const templateDayBtnEl = document.createElement("button");
  templateDayEl.className = "vanilla-calendar-day";
  templateDayBtnEl.className = "vanilla-calendar-day__btn";
  templateDayBtnEl.type = "button";
  if (["single", "multiple", "multiple-ranged"].includes(self.settings.selection.day)) {
    daysEl.classList.add("vanilla-calendar-days_selecting");
  }
  daysEl.innerHTML = "";
  const setDayModifier = (dayBtnEl, dayID, date, currentMonth) => {
    if (self.settings.visibility.weekend && (dayID === 0 || dayID === 6)) {
      dayBtnEl.classList.add("vanilla-calendar-day__btn_weekend");
    }
    if (Array.isArray(self.settings.selected.holidays)) {
      self.settings.selected.holidays.forEach((holiday) => {
        if (holiday === date) {
          dayBtnEl.classList.add("vanilla-calendar-day__btn_holiday");
        }
      });
    }
    let thisToday = self.date.today.getDate();
    let thisMonth = self.date.today.getMonth() + 1;
    thisToday = thisToday < 10 ? `0${thisToday}` : thisToday;
    thisMonth = thisMonth < 10 ? `0${thisMonth}` : thisMonth;
    const thisDay = `${self.date.today.getFullYear()}-${thisMonth}-${thisToday}`;
    if (self.settings.visibility.today && dayBtnEl.dataset.calendarDay === thisDay) {
      dayBtnEl.classList.add("vanilla-calendar-day__btn_today");
    }
    if (self.selectedDates && self.selectedDates.indexOf(date) === 0) {
      dayBtnEl.classList.add("vanilla-calendar-day__btn_selected");
    } else if (self.selectedDates && self.selectedDates[0] && self.selectedDates.indexOf(date) === self.selectedDates.length - 1) {
      dayBtnEl.classList.add("vanilla-calendar-day__btn_selected");
    } else if (self.selectedDates && self.selectedDates.indexOf(date) > 0 && self.settings.selection.day === "multiple-ranged") {
      dayBtnEl.classList.add("vanilla-calendar-day__btn_selected");
      dayBtnEl.classList.add("vanilla-calendar-day__btn_intermediate");
    } else if (self.selectedDates && self.selectedDates.indexOf(date) > 0) {
      dayBtnEl.classList.add("vanilla-calendar-day__btn_selected");
    }
    if (self.settings.range.min > date || self.settings.range.max < date) {
      dayBtnEl.classList.add("vanilla-calendar-day__btn_disabled");
      dayBtnEl.tabIndex = -1;
    }
    if (!self.settings.selection.month && !currentMonth) {
      dayBtnEl.classList.add("vanilla-calendar-day__btn_disabled");
      dayBtnEl.tabIndex = -1;
    }
    if (!self.settings.selection.year && new Date(date).getFullYear() !== self.selectedYear) {
      dayBtnEl.classList.add("vanilla-calendar-day__btn_disabled");
      dayBtnEl.tabIndex = -1;
    }
    if (Array.isArray(self.settings.range.disabled)) {
      self.settings.range.disabled.forEach((dateDisabled) => {
        if (dateDisabled === date) {
          dayBtnEl.classList.add("vanilla-calendar-day__btn_disabled");
          dayBtnEl.tabIndex = -1;
        }
      });
    } else if (Array.isArray(self.settings.range.enabled)) {
      dayBtnEl.classList.add("vanilla-calendar-day__btn_disabled");
      dayBtnEl.tabIndex = -1;
      self.settings.range.enabled.forEach((dateEnabled) => {
        if (dateEnabled === date) {
          dayBtnEl.classList.remove("vanilla-calendar-day__btn_disabled");
          dayBtnEl.tabIndex = 0;
        }
      });
    }
  };
  const createDay = (dayText, dayID, date, currentMonth, modifier) => {
    const dayEl = templateDayEl.cloneNode(true);
    const dayBtnEl = templateDayBtnEl.cloneNode(true);
    if (dayEl instanceof HTMLElement && dayBtnEl instanceof HTMLElement) {
      if (modifier)
        dayBtnEl.classList.add(modifier);
      dayBtnEl.innerText = dayText;
      dayBtnEl.dataset.calendarDay = date;
      setDayModifier(dayBtnEl, dayID, date, currentMonth);
      dayEl.append(dayBtnEl);
      daysEl.append(dayEl);
    }
  };
  const prevMonth = () => {
    if (self.selectedMonth === void 0 || self.selectedYear === void 0)
      return;
    const prevMonthDays = new Date(Date.UTC(self.selectedYear, self.selectedMonth, 0)).getUTCDate();
    let day = prevMonthDays - firstDayWeek;
    let year = self.selectedYear;
    let month = self.selectedMonth;
    if (self.selectedMonth === 0) {
      month = self.locale.months.length;
      year = self.selectedYear - 1;
    } else if (self.selectedMonth < 10) {
      month = `0${self.selectedMonth}`;
    }
    for (let i = 0; i < firstDayWeek; i++) {
      day += 1;
      const date = `${year}-${month}-${day}`;
      const dayIDCurrent = new Date(Date.UTC(self.selectedYear, self.selectedMonth, day - 1));
      const prevMonthID = dayIDCurrent.getUTCMonth() - 1;
      const dayID = new Date(Date.UTC(self.selectedYear, prevMonthID, day)).getUTCDay();
      createDay(String(day), dayID, date, false, "vanilla-calendar-day__btn_prev");
    }
  };
  const selectedMonth = () => {
    if (self.selectedMonth === void 0 || self.selectedYear === void 0)
      return;
    for (let i = 1; i <= daysSelectedMonth; i++) {
      const day = new Date(Date.UTC(self.selectedYear, self.selectedMonth, i));
      const date = (0,_generateDate__WEBPACK_IMPORTED_MODULE_2__["default"])(day);
      const dayID = day.getUTCDay();
      createDay(String(i), dayID, date, true, null);
    }
  };
  const nextMonth = () => {
    if (self.selectedMonth === void 0 || self.selectedYear === void 0)
      return;
    const total = firstDayWeek + daysSelectedMonth;
    const rows = Math.ceil(total / self.locale.weekday.length);
    const nextDays = self.locale.weekday.length * rows - total;
    let year = self.selectedYear;
    let month = String(self.selectedMonth + 2);
    if (self.selectedMonth + 1 === self.locale.months.length) {
      month = "01";
      year = self.selectedYear + 1;
    } else if (self.selectedMonth + 2 < 10) {
      month = `0${self.selectedMonth + 2}`;
    }
    for (let i = 1; i <= nextDays; i++) {
      const day = i < 10 ? `0${i}` : String(i);
      const date = `${year}-${month}-${day}`;
      const dayIDCurrent = new Date(Date.UTC(self.selectedYear, self.selectedMonth, i));
      const nextMonthID = dayIDCurrent.getUTCMonth() + 1;
      const dayID = new Date(Date.UTC(self.selectedYear, nextMonthID, i)).getUTCDay();
      createDay(String(i), dayID, date, false, "vanilla-calendar-day__btn_next");
    }
  };
  prevMonth();
  selectedMonth();
  nextMonth();
  (0,_createPopup__WEBPACK_IMPORTED_MODULE_0__["default"])(self, daysEl);
  (0,_createWeekNumbers__WEBPACK_IMPORTED_MODULE_1__["default"])(self, firstDayWeek, daysSelectedMonth);
};
/* harmony default export */ const __WEBPACK_DEFAULT_EXPORT__ = (createDays);


/***/ }),

/***/ "./src/scripts/methods/createHeader.ts":
/*!*********************************************!*\
  !*** ./src/scripts/methods/createHeader.ts ***!
  \*********************************************/
/***/ ((__unused_webpack_module, __webpack_exports__, __webpack_require__) => {

__webpack_require__.r(__webpack_exports__);
/* harmony export */ __webpack_require__.d(__webpack_exports__, {
/* harmony export */   "default": () => (__WEBPACK_DEFAULT_EXPORT__)
/* harmony export */ });

const createHeader = (self) => {
  const headerContent = self.HTMLElement.querySelector(".vanilla-calendar-header__content");
  if (self.selectedMonth === void 0 || self.selectedYear === void 0 || !headerContent)
    return;
  const monthDisabled = !self.settings.selection.month ? " vanilla-calendar-month_disabled" : "";
  const yearDisabled = !self.settings.selection.year ? " vanilla-calendar-year_disabled" : "";
  self.settings.selection.year = self.settings.selection.year;
  const month = `
	<button type="button"
		tabindex="${self.settings.selection.month ? 0 : -1}"
		class="vanilla-calendar-month${monthDisabled}"
		data-calendar-selected-month="${self.selectedMonth}">
		${self.locale.months[self.selectedMonth]}
	</button>`.replace(/[\n\t]/g, "");
  const year = `
	<button type="button"
		tabindex="${self.settings.selection.year ? 0 : -1}"
		class="vanilla-calendar-year${yearDisabled}"
		data-calendar-selected-year="${self.selectedYear}">
		${self.selectedYear}
	</button>`.replace(/[\n\t]/g, "");
  let templateHeader = self.settings.visibility.templateHeader.replace("%M", month);
  templateHeader = templateHeader.replace("%Y", year);
  headerContent.innerHTML = templateHeader;
};
/* harmony default export */ const __WEBPACK_DEFAULT_EXPORT__ = (createHeader);


/***/ }),

/***/ "./src/scripts/methods/createMonths.ts":
/*!*********************************************!*\
  !*** ./src/scripts/methods/createMonths.ts ***!
  \*********************************************/
/***/ ((__unused_webpack_module, __webpack_exports__, __webpack_require__) => {

__webpack_require__.r(__webpack_exports__);
/* harmony export */ __webpack_require__.d(__webpack_exports__, {
/* harmony export */   "default": () => (__WEBPACK_DEFAULT_EXPORT__)
/* harmony export */ });
/* harmony import */ var _createDOM__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ./createDOM */ "./src/scripts/methods/createDOM.ts");
/* harmony import */ var _createHeader__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ./createHeader */ "./src/scripts/methods/createHeader.ts");



const createMonths = (self) => {
  self.currentType = "month";
  (0,_createDOM__WEBPACK_IMPORTED_MODULE_0__["default"])(self);
  (0,_createHeader__WEBPACK_IMPORTED_MODULE_1__["default"])(self);
  const monthsEl = self.HTMLElement.querySelector(".vanilla-calendar-months");
  if (self.selectedMonth === void 0 || self.selectedYear === void 0 || !self.dateMin || !self.dateMax || !monthsEl)
    return;
  if (self.settings.selection.month)
    monthsEl.classList.add("vanilla-calendar-months_selecting");
  const templateMonthEl = document.createElement("button");
  templateMonthEl.type = "button";
  templateMonthEl.className = "vanilla-calendar-months__month";
  for (let i = 0; i < self.locale.months.length; i++) {
    const month = self.locale.months[i];
    const monthEl = templateMonthEl.cloneNode(true);
    if (monthEl instanceof HTMLElement) {
      if (i === self.selectedMonth) {
        monthEl.classList.add("vanilla-calendar-months__month_selected");
      }
      if (i < self.dateMin.getUTCMonth() && self.selectedYear === self.dateMin.getUTCFullYear()) {
        monthEl.classList.add("vanilla-calendar-months__month_disabled");
        monthEl.tabIndex = -1;
      }
      if (i > self.dateMax.getUTCMonth() && self.selectedYear === self.dateMax.getUTCFullYear()) {
        monthEl.classList.add("vanilla-calendar-months__month_disabled");
        monthEl.tabIndex = -1;
      }
      monthEl.dataset.calendarMonth = String(i);
      monthEl.title = `${month}`;
      monthEl.innerText = `${self.settings.visibility.monthShort ? month.substring(0, 3) : month}`;
      monthsEl.append(monthEl);
    }
  }
};
/* harmony default export */ const __WEBPACK_DEFAULT_EXPORT__ = (createMonths);


/***/ }),

/***/ "./src/scripts/methods/createPopup.ts":
/*!********************************************!*\
  !*** ./src/scripts/methods/createPopup.ts ***!
  \********************************************/
/***/ ((__unused_webpack_module, __webpack_exports__, __webpack_require__) => {

__webpack_require__.r(__webpack_exports__);
/* harmony export */ __webpack_require__.d(__webpack_exports__, {
/* harmony export */   "default": () => (__WEBPACK_DEFAULT_EXPORT__)
/* harmony export */ });

const createPopup = (self, daysEl) => {
  if (!self.popups)
    return;
  for (const day in self.popups) {
    if (Object.hasOwnProperty.call(self.popups, day)) {
      const dayBtnEl = daysEl.querySelector(`[data-calendar-day="${day}"]`);
      if (dayBtnEl) {
        const dayInfo = self.popups[day];
        dayBtnEl.classList.add(dayInfo.modifier);
        dayBtnEl.parentNode.innerHTML += `<div class="vanilla-calendar-day__popup">${dayInfo.html}</div>`;
      }
    }
  }
};
/* harmony default export */ const __WEBPACK_DEFAULT_EXPORT__ = (createPopup);


/***/ }),

/***/ "./src/scripts/methods/createTime.ts":
/*!*******************************************!*\
  !*** ./src/scripts/methods/createTime.ts ***!
  \*******************************************/
/***/ ((__unused_webpack_module, __webpack_exports__, __webpack_require__) => {

__webpack_require__.r(__webpack_exports__);
/* harmony export */ __webpack_require__.d(__webpack_exports__, {
/* harmony export */   "default": () => (__WEBPACK_DEFAULT_EXPORT__)
/* harmony export */ });
/* harmony import */ var _controlTime__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ./controlTime */ "./src/scripts/methods/controlTime.ts");
/* harmony import */ var _transformTime24__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ./transformTime24 */ "./src/scripts/methods/transformTime24.ts");



const createTime = (self) => {
  const timeEl = self.HTMLElement.querySelector(".vanilla-calendar-time");
  if (!timeEl)
    return;
  const keepingTime = self.settings.selection.time === true ? 12 : self.settings.selection.time;
  const range = self.settings.selection.controlTime === "range";
  timeEl.innerHTML = `
	<div class="vanilla-calendar-time__content">
		<label class="vanilla-calendar-time__hours">
			<input type="text"
				name="hours"
				maxlength="2"
				value="${self.selectedHours}"
				${range ? "disabled" : ""}>
		</label>
		<label class="vanilla-calendar-time__minutes">
			<input type="text"
				name="minutes"
				maxlength="2"
				value="${self.selectedMinutes}"
				${range ? "disabled" : ""}>
		</label>
		${keepingTime === 12 ? `
		<button type="button"
			class="vanilla-calendar-time__keeping"
			${range ? "disabled" : ""}>${self.selectedKeeping}</button>
		` : ""}
	</div>
	<div class="vanilla-calendar-time__ranges">
		<label class="vanilla-calendar-time__range">
			<input type="range"
				name="hours"
				min="0"
				max="23"
				step="${self.settings.selection.stepHours}"
				value="${self.selectedKeeping ? (0,_transformTime24__WEBPACK_IMPORTED_MODULE_1__["default"])(self.selectedHours, self.selectedKeeping) : self.selectedHours}">
		</label>
		<label class="vanilla-calendar-time__range">
			<input type="range"
				name="minutes"
				min="0"
				max="59"
				step="${self.settings.selection.stepMinutes}"
				value="${self.selectedMinutes}">
		</label>
	</div>`;
  (0,_controlTime__WEBPACK_IMPORTED_MODULE_0__["default"])(self, keepingTime);
};
/* harmony default export */ const __WEBPACK_DEFAULT_EXPORT__ = (createTime);


/***/ }),

/***/ "./src/scripts/methods/createWeek.ts":
/*!*******************************************!*\
  !*** ./src/scripts/methods/createWeek.ts ***!
  \*******************************************/
/***/ ((__unused_webpack_module, __webpack_exports__, __webpack_require__) => {

__webpack_require__.r(__webpack_exports__);
/* harmony export */ __webpack_require__.d(__webpack_exports__, {
/* harmony export */   "default": () => (__WEBPACK_DEFAULT_EXPORT__)
/* harmony export */ });

const createWeek = (self) => {
  const weekday = [...self.locale.weekday];
  if (!weekday[0])
    return;
  const weekEl = self.HTMLElement.querySelector(".vanilla-calendar-week");
  const templateWeekDayEl = document.createElement("b");
  templateWeekDayEl.className = "vanilla-calendar-week__day";
  if (self.settings.iso8601)
    weekday.push(weekday.shift());
  if (weekEl instanceof HTMLElement) {
    weekEl.innerHTML = "";
    for (let i = 0; i < weekday.length; i++) {
      const weekDayName = weekday[i];
      const weekDayEl = templateWeekDayEl.cloneNode(true);
      if (weekDayEl instanceof HTMLElement) {
        if (self.settings.visibility.weekend && self.settings.iso8601) {
          if (i === 5 || i === 6) {
            weekDayEl.classList.add("vanilla-calendar-week__day_weekend");
          }
        } else if (self.settings.visibility.weekend && !self.settings.iso8601) {
          if (i === 0 || i === 6) {
            weekDayEl.classList.add("vanilla-calendar-week__day_weekend");
          }
        }
        weekDayEl.innerText = `${weekDayName}`;
        weekEl.append(weekDayEl);
      }
    }
  }
};
/* harmony default export */ const __WEBPACK_DEFAULT_EXPORT__ = (createWeek);


/***/ }),

/***/ "./src/scripts/methods/createWeekNumbers.ts":
/*!**************************************************!*\
  !*** ./src/scripts/methods/createWeekNumbers.ts ***!
  \**************************************************/
/***/ ((__unused_webpack_module, __webpack_exports__, __webpack_require__) => {

__webpack_require__.r(__webpack_exports__);
/* harmony export */ __webpack_require__.d(__webpack_exports__, {
/* harmony export */   "default": () => (__WEBPACK_DEFAULT_EXPORT__)
/* harmony export */ });
/* harmony import */ var _getWeekNumber__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ./getWeekNumber */ "./src/scripts/methods/getWeekNumber.ts");


const createWeekNumbers = (self, firstDayWeek, daysSelectedMonth) => {
  if (!self.settings.visibility.weekNumbers)
    return;
  const weekNumbersEl = self.HTMLElement.querySelector(".vanilla-calendar-week-numbers");
  const daysBtnEl = self.HTMLElement.querySelectorAll(".vanilla-calendar-day__btn");
  if (weekNumbersEl instanceof HTMLElement) {
    const countWeek = Math.ceil((firstDayWeek + daysSelectedMonth) / 7);
    const templateWeekNumberEl = document.createElement("span");
    templateWeekNumberEl.className = "vanilla-calendar-week-number";
    weekNumbersEl.innerHTML = "";
    for (let i = 0; i < countWeek; i++) {
      const weekNumber = (0,_getWeekNumber__WEBPACK_IMPORTED_MODULE_0__["default"])(daysBtnEl[i * 7].dataset.calendarDay);
      if (!weekNumber)
        return;
      const weekNumberEl = templateWeekNumberEl.cloneNode(true);
      if (weekNumberEl instanceof HTMLElement) {
        weekNumberEl.innerText = `${weekNumber.week}`;
        weekNumberEl.dataset.calendarYearWeek = `${weekNumber.year}`;
        weekNumbersEl.append(weekNumberEl);
      }
    }
  }
};
/* harmony default export */ const __WEBPACK_DEFAULT_EXPORT__ = (createWeekNumbers);


/***/ }),

/***/ "./src/scripts/methods/createYears.ts":
/*!********************************************!*\
  !*** ./src/scripts/methods/createYears.ts ***!
  \********************************************/
/***/ ((__unused_webpack_module, __webpack_exports__, __webpack_require__) => {

__webpack_require__.r(__webpack_exports__);
/* harmony export */ __webpack_require__.d(__webpack_exports__, {
/* harmony export */   "default": () => (__WEBPACK_DEFAULT_EXPORT__)
/* harmony export */ });
/* harmony import */ var _controlArrows__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ./controlArrows */ "./src/scripts/methods/controlArrows.ts");
/* harmony import */ var _createDOM__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ./createDOM */ "./src/scripts/methods/createDOM.ts");
/* harmony import */ var _createHeader__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! ./createHeader */ "./src/scripts/methods/createHeader.ts");




const createYears = (self) => {
  if (self.viewYear === void 0 || !self.dateMin || !self.dateMax)
    return;
  self.currentType = "year";
  (0,_createDOM__WEBPACK_IMPORTED_MODULE_1__["default"])(self);
  (0,_createHeader__WEBPACK_IMPORTED_MODULE_2__["default"])(self);
  (0,_controlArrows__WEBPACK_IMPORTED_MODULE_0__["default"])(self);
  const yearsEl = self.HTMLElement.querySelector(".vanilla-calendar-years");
  if (!yearsEl)
    return;
  if (self.settings.selection.year)
    yearsEl.classList.add("vanilla-calendar-years_selecting");
  const templateYearEl = document.createElement("button");
  templateYearEl.type = "button";
  templateYearEl.className = "vanilla-calendar-years__year";
  self.viewYear = self.viewYear - 0;
  self.selectedYear = self.selectedYear - 0;
  for (let i = self.viewYear - 7; i < self.viewYear + 8; i++) {
    const year = i;
    const yearEl = templateYearEl.cloneNode(true);
    if (yearEl instanceof HTMLElement) {
      if (year === self.selectedYear) {
        yearEl.classList.add("vanilla-calendar-years__year_selected");
      }
      if (year < self.dateMin.getUTCFullYear()) {
        yearEl.classList.add("vanilla-calendar-years__year_disabled");
        yearEl.tabIndex = -1;
      }
      if (year > self.dateMax.getUTCFullYear()) {
        yearEl.classList.add("vanilla-calendar-years__year_disabled");
        yearEl.tabIndex = -1;
      }
      yearEl.dataset.calendarYear = String(year);
      yearEl.innerText = `${year}`;
      yearsEl.append(yearEl);
    }
  }
};
/* harmony default export */ const __WEBPACK_DEFAULT_EXPORT__ = (createYears);


/***/ }),

/***/ "./src/scripts/methods/generateDate.ts":
/*!*********************************************!*\
  !*** ./src/scripts/methods/generateDate.ts ***!
  \*********************************************/
/***/ ((__unused_webpack_module, __webpack_exports__, __webpack_require__) => {

__webpack_require__.r(__webpack_exports__);
/* harmony export */ __webpack_require__.d(__webpack_exports__, {
/* harmony export */   "default": () => (__WEBPACK_DEFAULT_EXPORT__)
/* harmony export */ });

const generateDate = (date) => {
  const year = date.getUTCFullYear();
  let month = date.getUTCMonth() + 1;
  let day = date.getUTCDate();
  month = month < 10 ? `0${month}` : month;
  day = day < 10 ? `0${day}` : day;
  return `${year}-${month}-${day}`;
};
/* harmony default export */ const __WEBPACK_DEFAULT_EXPORT__ = (generateDate);


/***/ }),

/***/ "./src/scripts/methods/getLocale.ts":
/*!******************************************!*\
  !*** ./src/scripts/methods/getLocale.ts ***!
  \******************************************/
/***/ ((__unused_webpack_module, __webpack_exports__, __webpack_require__) => {

__webpack_require__.r(__webpack_exports__);
/* harmony export */ __webpack_require__.d(__webpack_exports__, {
/* harmony export */   "default": () => (__WEBPACK_DEFAULT_EXPORT__)
/* harmony export */ });

const getLocale = (self) => {
  if (self.settings.lang === "define")
    return;
  self.locale.weekday = [];
  for (let i = 0; i < 7; i++) {
    let weekday = new Date(0, 0, i).toLocaleString(self.settings.lang, { weekday: "short" });
    weekday = `${weekday.charAt(0).toUpperCase()}${weekday.substring(1, weekday.length)}`;
    weekday = weekday.replace(/\./, "");
    self.locale.weekday.push(weekday);
  }
  self.locale.months = [];
  for (let i = 0; i < 12; i++) {
    let month = new Date(0, i).toLocaleString(self.settings.lang, { month: "long" });
    month = `${month.charAt(0).toUpperCase()}${month.substring(1, month.length)}`;
    month = month.replace(/\./, "");
    self.locale.months.push(month);
  }
};
/* harmony default export */ const __WEBPACK_DEFAULT_EXPORT__ = (getLocale);


/***/ }),

/***/ "./src/scripts/methods/getWeekNumber.ts":
/*!**********************************************!*\
  !*** ./src/scripts/methods/getWeekNumber.ts ***!
  \**********************************************/
/***/ ((__unused_webpack_module, __webpack_exports__, __webpack_require__) => {

__webpack_require__.r(__webpack_exports__);
/* harmony export */ __webpack_require__.d(__webpack_exports__, {
/* harmony export */   "default": () => (__WEBPACK_DEFAULT_EXPORT__)
/* harmony export */ });

const getWeekNumber = (date) => {
  if (!date)
    return null;
  const day = new Date(date).getUTCDate();
  const month = new Date(date).getUTCMonth();
  const year = new Date(date).getUTCFullYear();
  const correctDate = new Date(year, month, day);
  const yearStart = new Date(Date.UTC(correctDate.getUTCFullYear(), 0, 1));
  const weekNumber = Math.ceil(((+correctDate - +yearStart) / 864e5 + 1) / 7);
  return {
    year: correctDate.getUTCFullYear(),
    week: weekNumber
  };
};
/* harmony default export */ const __WEBPACK_DEFAULT_EXPORT__ = (getWeekNumber);


/***/ }),

/***/ "./src/scripts/methods/initCalendar.ts":
/*!*********************************************!*\
  !*** ./src/scripts/methods/initCalendar.ts ***!
  \*********************************************/
/***/ ((__unused_webpack_module, __webpack_exports__, __webpack_require__) => {

__webpack_require__.r(__webpack_exports__);
/* harmony export */ __webpack_require__.d(__webpack_exports__, {
/* harmony export */   "default": () => (__WEBPACK_DEFAULT_EXPORT__)
/* harmony export */ });
/* harmony import */ var _updateCalendar__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ./updateCalendar */ "./src/scripts/methods/updateCalendar.ts");
/* harmony import */ var _clickCalendar__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ./clickCalendar */ "./src/scripts/methods/clickCalendar.ts");



const initCalendar = (self) => {
  if (!self.HTMLElement)
    return;
  (0,_updateCalendar__WEBPACK_IMPORTED_MODULE_0__["default"])(self);
  (0,_clickCalendar__WEBPACK_IMPORTED_MODULE_1__["default"])(self);
};
/* harmony default export */ const __WEBPACK_DEFAULT_EXPORT__ = (initCalendar);


/***/ }),

/***/ "./src/scripts/methods/setVariablesDates.ts":
/*!**************************************************!*\
  !*** ./src/scripts/methods/setVariablesDates.ts ***!
  \**************************************************/
/***/ ((__unused_webpack_module, __webpack_exports__, __webpack_require__) => {

__webpack_require__.r(__webpack_exports__);
/* harmony export */ __webpack_require__.d(__webpack_exports__, {
/* harmony export */   "default": () => (__WEBPACK_DEFAULT_EXPORT__)
/* harmony export */ });
/* harmony import */ var _transformTime12__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ./transformTime12 */ "./src/scripts/methods/transformTime12.ts");


const setVariablesDates = (self) => {
  if (self.settings.selected.dates !== null) {
    self.selectedDates = self.settings.selected.dates;
  } else {
    self.selectedDates = [];
  }
  if (self.settings.selected.month !== null && self.settings.selected.month >= 0 && self.settings.selected.month < 12) {
    self.selectedMonth = self.settings.selected.month;
  } else {
    self.selectedMonth = self.date.today.getMonth();
  }
  if (self.settings.selected.year !== null && self.settings.selected.year >= 0 && self.settings.selected.year <= 9999) {
    self.selectedYear = self.settings.selected.year - 0;
  } else {
    self.selectedYear = self.date.today.getFullYear() - 0;
  }
  self.viewYear = self.selectedYear;
  self.dateMin = self.settings.visibility.disabled ? new Date(self.date.min) : new Date(self.settings.range.min);
  self.dateMax = self.settings.visibility.disabled ? new Date(self.date.max) : new Date(self.settings.range.max);
  const time12 = self.settings.selection.time === true || self.settings.selection.time === 12;
  if (time12 || self.settings.selection.time === 24) {
    if (typeof self.settings.selected.time === "string") {
      const regExr = time12 ? /^([0-9]|0[1-9]|1[0-2]):([0-5][0-9])|(AM|PM)/g : /^([0-1]?[0-9]|2[0-3]):([0-5][0-9])/g;
      self.settings.selected.time.replace(regExr, (_, p1, p2, p3) => {
        if (p1 && p2) {
          self.userTime = true;
          self.selectedHours = p1;
          self.selectedMinutes = p2;
        }
        if (p3 && time12) {
          self.selectedKeeping = p3;
        } else if (time12) {
          self.selectedKeeping = "AM";
        }
        return "";
      });
    }
    if (!self.userTime && time12) {
      self.selectedHours = (0,_transformTime12__WEBPACK_IMPORTED_MODULE_0__["default"])(String(self.date.today.getHours()));
      self.selectedMinutes = String(self.date.today.getMinutes());
      self.selectedKeeping = Number(self.date.today.getHours()) > 12 ? "PM" : "AM";
    } else if (!self.userTime) {
      self.selectedHours = String(self.date.today.getHours());
      self.selectedMinutes = String(self.date.today.getMinutes());
    }
    self.selectedHours = Number(self.selectedHours) < 10 ? `0${Number(self.selectedHours)}` : `${self.selectedHours}`;
    self.selectedMinutes = Number(self.selectedMinutes) < 10 ? `0${Number(self.selectedMinutes)}` : `${self.selectedMinutes}`;
    self.selectedTime = `${self.selectedHours}:${self.selectedMinutes}${self.selectedKeeping ? ` ${self.selectedKeeping}` : ""}`;
  } else if (self.settings.selection.time) {
    self.settings.selection.time = false;
    console.error("The value of the time property can be: false, true, 12 or 24.");
  }
};
/* harmony default export */ const __WEBPACK_DEFAULT_EXPORT__ = (setVariablesDates);


/***/ }),

/***/ "./src/scripts/methods/transformTime12.ts":
/*!************************************************!*\
  !*** ./src/scripts/methods/transformTime12.ts ***!
  \************************************************/
/***/ ((__unused_webpack_module, __webpack_exports__, __webpack_require__) => {

__webpack_require__.r(__webpack_exports__);
/* harmony export */ __webpack_require__.d(__webpack_exports__, {
/* harmony export */   "default": () => (__WEBPACK_DEFAULT_EXPORT__)
/* harmony export */ });

const transformTime12 = (hour) => {
  const oldHour = Number(hour);
  let newHour = String(oldHour);
  if (oldHour === 0) {
    newHour = "12";
  } else if (oldHour === 13) {
    newHour = "01";
  } else if (oldHour === 14) {
    newHour = "02";
  } else if (oldHour === 15) {
    newHour = "03";
  } else if (oldHour === 16) {
    newHour = "04";
  } else if (oldHour === 17) {
    newHour = "05";
  } else if (oldHour === 18) {
    newHour = "06";
  } else if (oldHour === 19) {
    newHour = "07";
  } else if (oldHour === 20) {
    newHour = "08";
  } else if (oldHour === 21) {
    newHour = "09";
  } else if (oldHour === 22) {
    newHour = "10";
  } else if (oldHour === 23) {
    newHour = "11";
  }
  return newHour;
};
/* harmony default export */ const __WEBPACK_DEFAULT_EXPORT__ = (transformTime12);


/***/ }),

/***/ "./src/scripts/methods/transformTime24.ts":
/*!************************************************!*\
  !*** ./src/scripts/methods/transformTime24.ts ***!
  \************************************************/
/***/ ((__unused_webpack_module, __webpack_exports__, __webpack_require__) => {

__webpack_require__.r(__webpack_exports__);
/* harmony export */ __webpack_require__.d(__webpack_exports__, {
/* harmony export */   "default": () => (__WEBPACK_DEFAULT_EXPORT__)
/* harmony export */ });

const transformTime24 = (hour, keeping) => {
  const oldHour = Number(hour);
  let newHour = String(oldHour);
  if (keeping === "AM") {
    if (oldHour === 12) {
      newHour = "00";
    }
  } else if (keeping === "PM") {
    if (oldHour === 1) {
      newHour = "13";
    } else if (oldHour === 2) {
      newHour = "14";
    } else if (oldHour === 3) {
      newHour = "15";
    } else if (oldHour === 4) {
      newHour = "16";
    } else if (oldHour === 5) {
      newHour = "17";
    } else if (oldHour === 6) {
      newHour = "18";
    } else if (oldHour === 7) {
      newHour = "19";
    } else if (oldHour === 8) {
      newHour = "20";
    } else if (oldHour === 9) {
      newHour = "21";
    } else if (oldHour === 10) {
      newHour = "22";
    } else if (oldHour === 11) {
      newHour = "23";
    }
  }
  return newHour;
};
/* harmony default export */ const __WEBPACK_DEFAULT_EXPORT__ = (transformTime24);


/***/ }),

/***/ "./src/scripts/methods/updateCalendar.ts":
/*!***********************************************!*\
  !*** ./src/scripts/methods/updateCalendar.ts ***!
  \***********************************************/
/***/ ((__unused_webpack_module, __webpack_exports__, __webpack_require__) => {

__webpack_require__.r(__webpack_exports__);
/* harmony export */ __webpack_require__.d(__webpack_exports__, {
/* harmony export */   "default": () => (__WEBPACK_DEFAULT_EXPORT__)
/* harmony export */ });
/* harmony import */ var _controlArrows__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ./controlArrows */ "./src/scripts/methods/controlArrows.ts");
/* harmony import */ var _createDays__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ./createDays */ "./src/scripts/methods/createDays.ts");
/* harmony import */ var _createDOM__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! ./createDOM */ "./src/scripts/methods/createDOM.ts");
/* harmony import */ var _createHeader__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! ./createHeader */ "./src/scripts/methods/createHeader.ts");
/* harmony import */ var _createMonths__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! ./createMonths */ "./src/scripts/methods/createMonths.ts");
/* harmony import */ var _createTime__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! ./createTime */ "./src/scripts/methods/createTime.ts");
/* harmony import */ var _createWeek__WEBPACK_IMPORTED_MODULE_6__ = __webpack_require__(/*! ./createWeek */ "./src/scripts/methods/createWeek.ts");
/* harmony import */ var _createYears__WEBPACK_IMPORTED_MODULE_7__ = __webpack_require__(/*! ./createYears */ "./src/scripts/methods/createYears.ts");
/* harmony import */ var _getLocale__WEBPACK_IMPORTED_MODULE_8__ = __webpack_require__(/*! ./getLocale */ "./src/scripts/methods/getLocale.ts");
/* harmony import */ var _setVariablesDates__WEBPACK_IMPORTED_MODULE_9__ = __webpack_require__(/*! ./setVariablesDates */ "./src/scripts/methods/setVariablesDates.ts");











const update = (self) => {
  (0,_setVariablesDates__WEBPACK_IMPORTED_MODULE_9__["default"])(self);
  (0,_getLocale__WEBPACK_IMPORTED_MODULE_8__["default"])(self);
  (0,_createDOM__WEBPACK_IMPORTED_MODULE_2__["default"])(self);
  (0,_createHeader__WEBPACK_IMPORTED_MODULE_3__["default"])(self);
  (0,_controlArrows__WEBPACK_IMPORTED_MODULE_0__["default"])(self);
  (0,_createTime__WEBPACK_IMPORTED_MODULE_5__["default"])(self);
  if (self.currentType === "default") {
    (0,_createWeek__WEBPACK_IMPORTED_MODULE_6__["default"])(self);
    (0,_createDays__WEBPACK_IMPORTED_MODULE_1__["default"])(self);
  } else if (self.currentType === "month") {
    (0,_createMonths__WEBPACK_IMPORTED_MODULE_4__["default"])(self);
  } else if (self.currentType === "year") {
    (0,_createYears__WEBPACK_IMPORTED_MODULE_7__["default"])(self);
  }
};
/* harmony default export */ const __WEBPACK_DEFAULT_EXPORT__ = (update);


/***/ }),

/***/ "./src/scripts/vanilla-calendar.ts":
/*!*****************************************!*\
  !*** ./src/scripts/vanilla-calendar.ts ***!
  \*****************************************/
/***/ ((__unused_webpack_module, __webpack_exports__, __webpack_require__) => {

__webpack_require__.r(__webpack_exports__);
/* harmony export */ __webpack_require__.d(__webpack_exports__, {
/* harmony export */   "default": () => (/* binding */ VanillaCalendar)
/* harmony export */ });
/* harmony import */ var _methods_initCalendar__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ./methods/initCalendar */ "./src/scripts/methods/initCalendar.ts");


class VanillaCalendar {
  constructor(selector, option) {
    this.init = () => (0,_methods_initCalendar__WEBPACK_IMPORTED_MODULE_0__["default"])(this);
    var _a, _b, _c, _d, _e, _f, _g, _h, _i, _j, _k, _l, _m, _n, _o, _p, _q, _r, _s, _t, _u, _v, _w, _x, _y, _z, _A, _B, _C, _D, _E, _F, _G, _H, _I, _J, _K, _L, _M, _N, _O, _P, _Q, _R, _S, _T, _U, _V, _W, _X, _Y, _Z, __, _$, _aa, _ba, _ca, _da, _ea, _fa, _ga, _ha, _ia, _ja, _ka, _la, _ma, _na, _oa, _pa, _qa, _ra, _sa, _ta, _ua, _va, _wa, _xa, _ya, _za, _Aa, _Ba, _Ca, _Da, _Ea, _Fa, _Ga, _Ha, _Ia, _Ja;
    this.HTMLElement = typeof selector === "string" ? document.querySelector(selector) : selector;
    if (!this.HTMLElement)
      return;
    this.type = (_a = option == null ? void 0 : option.type) != null ? _a : "default";
    this.date = {
      min: (_c = (_b = option == null ? void 0 : option.date) == null ? void 0 : _b.min) != null ? _c : "1970-01-01",
      max: (_e = (_d = option == null ? void 0 : option.date) == null ? void 0 : _d.max) != null ? _e : "2470-12-31",
      today: (_g = (_f = option == null ? void 0 : option.date) == null ? void 0 : _f.today) != null ? _g : new Date()
    };
    this.settings = {
      lang: (_i = (_h = option == null ? void 0 : option.settings) == null ? void 0 : _h.lang) != null ? _i : "en",
      iso8601: (_k = (_j = option == null ? void 0 : option.settings) == null ? void 0 : _j.iso8601) != null ? _k : true,
      range: {
        min: (_n = (_m = (_l = option == null ? void 0 : option.settings) == null ? void 0 : _l.range) == null ? void 0 : _m.min) != null ? _n : this.date.min,
        max: (_q = (_p = (_o = option == null ? void 0 : option.settings) == null ? void 0 : _o.range) == null ? void 0 : _p.max) != null ? _q : this.date.max,
        disabled: (_t = (_s = (_r = option == null ? void 0 : option.settings) == null ? void 0 : _r.range) == null ? void 0 : _s.disabled) != null ? _t : null,
        enabled: (_w = (_v = (_u = option == null ? void 0 : option.settings) == null ? void 0 : _u.range) == null ? void 0 : _v.enabled) != null ? _w : null
      },
      selection: {
        day: (_z = (_y = (_x = option == null ? void 0 : option.settings) == null ? void 0 : _x.selection) == null ? void 0 : _y.day) != null ? _z : "single",
        month: (_C = (_B = (_A = option == null ? void 0 : option.settings) == null ? void 0 : _A.selection) == null ? void 0 : _B.month) != null ? _C : true,
        year: (_F = (_E = (_D = option == null ? void 0 : option.settings) == null ? void 0 : _D.selection) == null ? void 0 : _E.year) != null ? _F : true,
        time: (_I = (_H = (_G = option == null ? void 0 : option.settings) == null ? void 0 : _G.selection) == null ? void 0 : _H.time) != null ? _I : false,
        controlTime: (_L = (_K = (_J = option == null ? void 0 : option.settings) == null ? void 0 : _J.selection) == null ? void 0 : _K.controlTime) != null ? _L : "all",
        stepHours: (_O = (_N = (_M = option == null ? void 0 : option.settings) == null ? void 0 : _M.selection) == null ? void 0 : _N.stepHours) != null ? _O : 1,
        stepMinutes: (_R = (_Q = (_P = option == null ? void 0 : option.settings) == null ? void 0 : _P.selection) == null ? void 0 : _Q.stepMinutes) != null ? _R : 1
      },
      selected: {
        dates: (_U = (_T = (_S = option == null ? void 0 : option.settings) == null ? void 0 : _S.selected) == null ? void 0 : _T.dates) != null ? _U : null,
        month: (_X = (_W = (_V = option == null ? void 0 : option.settings) == null ? void 0 : _V.selected) == null ? void 0 : _W.month) != null ? _X : null,
        year: (__ = (_Z = (_Y = option == null ? void 0 : option.settings) == null ? void 0 : _Y.selected) == null ? void 0 : _Z.year) != null ? __ : null,
        holidays: (_ba = (_aa = (_$ = option == null ? void 0 : option.settings) == null ? void 0 : _$.selected) == null ? void 0 : _aa.holidays) != null ? _ba : null,
        time: (_ea = (_da = (_ca = option == null ? void 0 : option.settings) == null ? void 0 : _ca.selected) == null ? void 0 : _da.time) != null ? _ea : null
      },
      visibility: {
        templateHeader: (_ha = (_ga = (_fa = option == null ? void 0 : option.settings) == null ? void 0 : _fa.visibility) == null ? void 0 : _ga.templateHeader) != null ? _ha : "%M %Y",
        monthShort: (_ka = (_ja = (_ia = option == null ? void 0 : option.settings) == null ? void 0 : _ia.visibility) == null ? void 0 : _ja.monthShort) != null ? _ka : true,
        weekNumbers: (_na = (_ma = (_la = option == null ? void 0 : option.settings) == null ? void 0 : _la.visibility) == null ? void 0 : _ma.weekNumbers) != null ? _na : false,
        weekend: (_qa = (_pa = (_oa = option == null ? void 0 : option.settings) == null ? void 0 : _oa.visibility) == null ? void 0 : _pa.weekend) != null ? _qa : true,
        today: (_ta = (_sa = (_ra = option == null ? void 0 : option.settings) == null ? void 0 : _ra.visibility) == null ? void 0 : _sa.today) != null ? _ta : true,
        disabled: (_wa = (_va = (_ua = option == null ? void 0 : option.settings) == null ? void 0 : _ua.visibility) == null ? void 0 : _va.disabled) != null ? _wa : false
      }
    };
    this.locale = {
      months: (_ya = (_xa = option == null ? void 0 : option.locale) == null ? void 0 : _xa.months) != null ? _ya : [],
      weekday: (_Aa = (_za = option == null ? void 0 : option.locale) == null ? void 0 : _za.weekday) != null ? _Aa : []
    };
    this.actions = {
      clickDay: (_Ca = (_Ba = option == null ? void 0 : option.actions) == null ? void 0 : _Ba.clickDay) != null ? _Ca : null,
      clickMonth: (_Ea = (_Da = option == null ? void 0 : option.actions) == null ? void 0 : _Da.clickMonth) != null ? _Ea : null,
      clickYear: (_Ga = (_Fa = option == null ? void 0 : option.actions) == null ? void 0 : _Fa.clickYear) != null ? _Ga : null,
      changeTime: (_Ia = (_Ha = option == null ? void 0 : option.actions) == null ? void 0 : _Ha.changeTime) != null ? _Ia : null
    };
    this.popups = (_Ja = option == null ? void 0 : option.popups) != null ? _Ja : null;
    this.currentType = this.type;
    this.selectedKeeping = null;
    this.userTime = false;
  }
}


/***/ }),

/***/ "./src/styles/vanilla-calendar.scss":
/*!******************************************!*\
  !*** ./src/styles/vanilla-calendar.scss ***!
  \******************************************/
/***/ ((__unused_webpack_module, __webpack_exports__, __webpack_require__) => {

__webpack_require__.r(__webpack_exports__);
// extracted by mini-css-extract-plugin


/***/ })

/******/ 	});
/************************************************************************/
/******/ 	// The module cache
/******/ 	var __webpack_module_cache__ = {};
/******/ 	
/******/ 	// The require function
/******/ 	function __webpack_require__(moduleId) {
/******/ 		// Check if module is in cache
/******/ 		var cachedModule = __webpack_module_cache__[moduleId];
/******/ 		if (cachedModule !== undefined) {
/******/ 			return cachedModule.exports;
/******/ 		}
/******/ 		// Create a new module (and put it into the cache)
/******/ 		var module = __webpack_module_cache__[moduleId] = {
/******/ 			// no module.id needed
/******/ 			// no module.loaded needed
/******/ 			exports: {}
/******/ 		};
/******/ 	
/******/ 		// Execute the module function
/******/ 		__webpack_modules__[moduleId](module, module.exports, __webpack_require__);
/******/ 	
/******/ 		// Return the exports of the module
/******/ 		return module.exports;
/******/ 	}
/******/ 	
/************************************************************************/
/******/ 	/* webpack/runtime/define property getters */
/******/ 	(() => {
/******/ 		// define getter functions for harmony exports
/******/ 		__webpack_require__.d = (exports, definition) => {
/******/ 			for(var key in definition) {
/******/ 				if(__webpack_require__.o(definition, key) && !__webpack_require__.o(exports, key)) {
/******/ 					Object.defineProperty(exports, key, { enumerable: true, get: definition[key] });
/******/ 				}
/******/ 			}
/******/ 		};
/******/ 	})();
/******/ 	
/******/ 	/* webpack/runtime/hasOwnProperty shorthand */
/******/ 	(() => {
/******/ 		__webpack_require__.o = (obj, prop) => (Object.prototype.hasOwnProperty.call(obj, prop))
/******/ 	})();
/******/ 	
/******/ 	/* webpack/runtime/make namespace object */
/******/ 	(() => {
/******/ 		// define __esModule on exports
/******/ 		__webpack_require__.r = (exports) => {
/******/ 			if(typeof Symbol !== 'undefined' && Symbol.toStringTag) {
/******/ 				Object.defineProperty(exports, Symbol.toStringTag, { value: 'Module' });
/******/ 			}
/******/ 			Object.defineProperty(exports, '__esModule', { value: true });
/******/ 		};
/******/ 	})();
/******/ 	
/************************************************************************/
var __webpack_exports__ = {};
// This entry need to be wrapped in an IIFE because it need to be isolated against other modules in the chunk.
(() => {
/*!**********************!*\
  !*** ./src/index.ts ***!
  \**********************/
__webpack_require__.r(__webpack_exports__);
/* harmony export */ __webpack_require__.d(__webpack_exports__, {
/* harmony export */   "default": () => (__WEBPACK_DEFAULT_EXPORT__)
/* harmony export */ });
/* harmony import */ var _scripts_vanilla_calendar__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ./scripts/vanilla-calendar */ "./src/scripts/vanilla-calendar.ts");
/* harmony import */ var _styles_vanilla_calendar_scss__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ./styles/vanilla-calendar.scss */ "./src/styles/vanilla-calendar.scss");



window.VanillaCalendar = _scripts_vanilla_calendar__WEBPACK_IMPORTED_MODULE_0__["default"];
/* harmony default export */ const __WEBPACK_DEFAULT_EXPORT__ = (_scripts_vanilla_calendar__WEBPACK_IMPORTED_MODULE_0__["default"]);

})();

/******/ 	return __webpack_exports__;
/******/ })()
;
});
//# sourceMappingURL=vanilla-calendar.js.map