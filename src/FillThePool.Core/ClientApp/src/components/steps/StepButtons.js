import 'date-fns';
import React from "react";
import { withStyles } from '@material-ui/core/styles';
import PropTypes from 'prop-types';
import Button from '@material-ui/core/Button';

const styles = theme => ({
	margin: {
		margin: theme.spacing.unit * 2,
	},
	padding: {
		padding: `0 ${theme.spacing.unit * 2}px`,
	},
	actionsContainer: {
		marginBottom: theme.spacing.unit * 2,
	},
	focusVisible: {},
});


class StepButtons extends React.Component {
	render() {
        const { classes, handleNext, activeStep, steps, handleBack, handleFinish } = this.props;
		return (
			<div>
				<Button
					disabled={activeStep === 0}
					onClick={handleBack}
					className={classes.button}
				>Back</Button>
				<Button
					variant="contained"
					color="primary"
                    onClick={activeStep === steps.length - 1 ? handleFinish : handleNext}
					className={classes.button}>
					{activeStep === steps.length - 1 ? 'Finish' : 'Next'}
				</Button>
			</div>
		)
	}
}

StepButtons.propTypes = {
	classes: PropTypes.object.isRequired,
    handleNext: PropTypes.func,
    handleBack: PropTypes.func,
    handleFinish: PropTypes.func
};

export default withStyles(styles)(StepButtons);