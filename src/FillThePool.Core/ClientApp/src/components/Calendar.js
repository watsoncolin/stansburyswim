import 'date-fns';
import React from "react";
import { DatePicker, MuiPickersUtilsProvider } from 'material-ui-pickers';
import PropTypes from 'prop-types';
import { withStyles } from '@material-ui/core/styles';
import { IconButton } from '@material-ui/core';
import DateFnsUtils from '@date-io/date-fns';
import * as moment from 'moment';
import clsx from "clsx";


class Calendar extends React.Component {
	state = {
		selectedDate: undefined,
		dates: {},
	};

	handleDateChange = date => {
        this.props.onSelect(date);
	};

	componentDidMount = () => {
		const dates = {};
		for (let lesson of this.props.lessonDates) {
			const date = moment(lesson)
			if (date.isAfter(moment())) {
				dates[date.dayOfYear()] = date;
			}
		}

		this.setState({ dates });
	}

	renderDay = (date, _, dayInCurrentMonth) => {
        const { classes } = this.props;
		let dateClone = moment(date);
		const lesson = this.state.dates[dateClone.dayOfYear()];

        let dayIsAvailable = false;
        if (lesson) {
            dayIsAvailable = true;
        }

		//const scheduledDay = moment(dateClone).date() % 3 === 0;
		const wrapperClassName = clsx({
			[classes.highlight]: dayIsAvailable && dayInCurrentMonth,
			//[classes.scheduledDay]: scheduledDay && dayInCurrentMonth,
			//[classes.both]: dayIsAvailable && scheduledDay && dayInCurrentMonth,
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
		const { selectedDate } = this.props;

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
    lessonDates: PropTypes.array.isRequired,
    selectedDate: PropTypes.object,
    onSelect: PropTypes.func,
};


const styles = theme => ({
    actionsContainer: {
        marginBottom: theme.spacing.unit * 2,
    },
    margin: {
        margin: theme.spacing.unit * 2,
	},
	padding: {
		padding: `0 ${theme.spacing.unit * 2}px`,
	},
	focusVisible: { },
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

export default withStyles(styles)(Calendar);