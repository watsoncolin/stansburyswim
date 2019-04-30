import ReactDOM from 'react-dom';
import React from 'react';
import Schedule from './components/Schedule';
import AdminSchedule from './components/AdminSchedule';
import jquery from 'jquery';
import 'babel-polyfill';
window.AdminSchedule = AdminSchedule;
window.Schedule = Schedule;
window.ReactDOM = ReactDOM;
window.React = React;
window.$ = jquery;





