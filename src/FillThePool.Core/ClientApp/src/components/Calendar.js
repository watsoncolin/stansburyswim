import 'date-fns';
import React from "react";
import { DatePicker, MuiPickersUtilsProvider } from 'material-ui-pickers';
import PropTypes from 'prop-types';
import { withStyles } from '@material-ui/core/styles';
import { IconButton } from '@material-ui/core';
import DateFnsUtils from '@date-io/date-fns';
import isSameDay from "date-fns/isSameDay";
import startOfWeek from "date-fns/startOfWeek";
import endOfWeek from "date-fns/endOfWeek";
import isWithinInterval from "date-fns/isWithinInterval";
import * as moment from 'moment';
import clsx from "clsx";
import format from "date-fns/format";


const styles = theme => ({
	dayWrapper: {
		position: "relative",
	},
	day: {
		width: 36,
		height: 36,
		fontSize: theme.typography.caption.fontSize,
		margin: "0 2px",
		color: "inherit",
		padding: 0,
	},
	customDayHighlight: {
		position: "absolute",
		top: 0,
		bottom: 0,
		left: "2px",
		right: "2px",
		border: `1px solid ${theme.palette.secondary.main}`,
		borderRadius: "50%",
	},
	nonCurrentMonthDay: {
		color: theme.palette.text.disabled,
	},
	highlightNonCurrentMonthDay: {
		color: "#676767",
	},
	highlight: {
		background: theme.palette.primary.main,
		color: theme.palette.common.white,
		borderRadius: "50%",
	},
	scheduledDay: {
		background: theme.palette.secondary.main,
		color: theme.palette.common.white,
		borderRadius: "50%",
	},
	both: {
		background: `repeating-linear-gradient(45deg,${theme.palette.primary.main},${theme.palette.primary.main} 10px,${theme.palette.secondary.main} 10px,${theme.palette.secondary.main} 20px)`
	}
});


class Calendar extends React.Component {
	state = {
		// The first commit of Material-UI
		selectedDate: new Date(),
	};

	handleDateChange = date => {
		this.setState({ selectedDate: date });
	};

	renderDay = (date, selectedDate, dayInCurrentMonth) => {
		const { classes } = this.props;
		let dateClone = moment(date);
		const dayIsAvailable = moment(dateClone).date() % 2 === 0;
		const scheduledDay = moment(dateClone).date() % 3 === 0;
		const wrapperClassName = clsx({
			[classes.highlight]: dayIsAvailable && dayInCurrentMonth,
			[classes.scheduledDay]: scheduledDay && dayInCurrentMonth,
			[classes.both]: dayIsAvailable && scheduledDay && dayInCurrentMonth,
		});

		const dayClassName = clsx(classes.day, {
			[classes.nonCurrentMonthDay]: !dayInCurrentMonth,
			[classes.highlightNonCurrentMonthDay]: !dayInCurrentMonth,
		});

		return (
			<div className={wrapperClassName}>
				<IconButton className={dayClassName}>
					<span> {moment(dateClone).format("D")} </span>
				</IconButton>
			</div>
		);
	};

	render() {
		const { selectedDate } = this.state;

		return (
			<MuiPickersUtilsProvider utils={DateFnsUtils}>
				<div className="picker">
					<DatePicker
						label="Lesson Calendar"
						value={selectedDate}
						onChange={this.handleDateChange}
						animateYearScrolling
						renderDay={this.renderDay}
					/>
				</div>
			</MuiPickersUtilsProvider>
		);
	}
}

Calendar.propTypes = {
	classes: PropTypes.object.isRequired,
};

export default withStyles(styles)(Calendar);