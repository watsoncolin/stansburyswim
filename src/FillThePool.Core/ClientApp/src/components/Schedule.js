import React from 'react';
import PropTypes from 'prop-types';
import { withStyles } from '@material-ui/core/styles';
import Stepper from '@material-ui/core/Stepper';
import Step from '@material-ui/core/Step';
import StepLabel from '@material-ui/core/StepLabel';
import StepContent from '@material-ui/core/StepContent';
import Button from '@material-ui/core/Button';
import Paper from '@material-ui/core/Paper';
import Typography from '@material-ui/core/Typography';
import Calendar from './Calendar';
import Tabs from '@material-ui/core/Tabs';
import Tab from '@material-ui/core/Tab';
import Table from '@material-ui/core/Table';
import TableBody from '@material-ui/core/TableBody';
import TableCell from '@material-ui/core/TableCell';
import TableHead from '@material-ui/core/TableHead';
import TableRow from '@material-ui/core/TableRow';

function TabContainer(props) {
	return (
		<Typography component="div" style={{ padding: 8 * 3 }}>
			{props.children}
		</Typography>
	);
}

TabContainer.propTypes = {
	children: PropTypes.node.isRequired,
};

const styles = theme => ({
	root: {
		width: '90%',
	},
	button: {
		marginTop: theme.spacing.unit,
		marginRight: theme.spacing.unit,
	},
	actionsContainer: {
		marginBottom: theme.spacing.unit * 2,
	},
	resetContainer: {
		padding: theme.spacing.unit * 3,
	},
});

function getSteps() {
	return ['Select day', 'Pick your instructor and time slot', 'Confirm'];
}

class Schedule extends React.Component {
	state = {
		activeStep: 0,
		value: 0,
	};

	handleNext = () => {
		this.setState(state => ({
			activeStep: state.activeStep + 1,
		}));
	};

	handleBack = () => {
		this.setState(state => ({
			activeStep: state.activeStep - 1,
		}));
	};

	handleReset = () => {
		this.setState({
			activeStep: 0,
		});
	};

	handleChange = (event, value) => {
		this.setState({ value });
	};

	render() {
		const { classes } = this.props;
		const steps = getSteps();
		const { activeStep } = this.state;

		const { value } = this.state;
		return (
			<div className={classes.root}>
				<Stepper activeStep={activeStep} orientation="vertical">
					<Step>
						<StepLabel>Select date</StepLabel>
						<StepContent>
							<Typography>Days with available lesson slots are Blue.  Days that you've scheduled lessons are Green.</Typography>
							<div className={classes.actionsContainer}>
								<Calendar />
								<div>
									<Button
										disabled={activeStep === 0}
										onClick={this.handleBack}
										className={classes.button}
									>Back</Button>
									<Button
										variant="contained"
										color="primary"
										onClick={this.handleNext}
										className={classes.button}>
										{activeStep === steps.length - 1 ? 'Finish' : 'Next'}
									</Button>
								</div>
							</div>
						</StepContent>
					</Step>
					<Step>
						<StepLabel>Pick your instructor and time slot</StepLabel>
						<StepContent>
							<Typography></Typography>
							<div className={classes.actionsContainer}>
								<Tabs value={value} onChange={this.handleChange}>
									<Tab label="Item One" />
									<Tab label="Item Two" />
									<Tab label="Item Three" />
								</Tabs>
								{value === 0 && <TabContainer>Item One</TabContainer>}
								{value === 1 && <TabContainer>Item Two</TabContainer>}
								{value === 2 && <TabContainer>Item Three</TabContainer>}
								<div>
									<Button
										disabled={activeStep === 0}
										onClick={this.handleBack}
										className={classes.button}
									>Back</Button>
									<Button
										variant="contained"
										color="primary"
										onClick={this.handleNext}
										className={classes.button}>
										{activeStep === steps.length - 1 ? 'Finish' : 'Next'}
									</Button>
								</div>
							</div>
						</StepContent>
					</Step>
					<Step>
						<StepLabel>Confirm</StepLabel>
						<StepContent>
							<Typography>How does this look?</Typography>
							<div className={classes.actionsContainer}>
								<Table>
									<TableHead>
										<TableRow>
											<TableCell>Date Time</TableCell>
											<TableCell align="right">Lane</TableCell>
											<TableCell align="right">Student</TableCell>
										</TableRow>
									</TableHead>
									<TableBody>
										<TableRow>
											<TableCell component="th" scope="row">
												03/21/2019 1:00pm
												</TableCell>
											<TableCell align="right">Lane 1</TableCell>
											<TableCell align="right">Archie</TableCell>
										</TableRow>
									</TableBody>
								</Table>

								<Button
									variant="contained"
									color="primary"
									onClick={this.handleNext}
									className={classes.button}>
									{activeStep === steps.length - 1 ? 'Finish' : 'Next'}
								</Button>
							</div>
						</StepContent>
					</Step>
				</Stepper>
				{activeStep === steps.length && (
					<Paper square elevation={0} className={classes.resetContainer}>
						<Typography>All steps completed - you&apos;re finished</Typography>
						<Button onClick={this.handleReset} className={classes.button}>
							Schedule another lesson
						</Button>
					</Paper>
				)}
			</div>
		);
	}
}

Schedule.propTypes = {
	classes: PropTypes.object,
};

export default withStyles(styles)(Schedule);