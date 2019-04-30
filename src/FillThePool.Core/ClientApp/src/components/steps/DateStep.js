import 'date-fns';
import React from "react";
import { withStyles } from '@material-ui/core/styles';
import PropTypes from 'prop-types';
import Typography from '@material-ui/core/Typography';
import Calendar from '../Calendar';
import StepButtons from './StepButtons';

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
	focusVisible: {},
});


class DateStep extends React.Component {
	render() {
		const { classes } = this.props;
		return (
			<div className={classes.actionsContainer}>
				<Typography>Days with available lesson slots are Blue.</Typography>
                <Calendar {...this.props} />
				<div>
					<br />
					<br />
					<StepButtons {...this.props} />
				</div>
			</div>
		)
	}
}

DateStep.propTypes = {
	classes: PropTypes.object.isRequired,
	onSelect: PropTypes.func,
	handleBack: PropTypes.func,
	handleNext: PropTypes.func,
    lessonDates: PropTypes.array.isRequired,
    selectedDate: PropTypes.object,
};

export default withStyles(styles)(DateStep);