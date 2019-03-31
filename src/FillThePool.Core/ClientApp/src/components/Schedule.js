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
import Badge from '@material-ui/core/Badge';
import Checkbox from '@material-ui/core/Checkbox';
import FormControl from '@material-ui/core/FormControl';
import Select from '@material-ui/core/Select';
import InputLabel from '@material-ui/core/InputLabel';
import MenuItem from '@material-ui/core/MenuItem';

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
	table: {
		width: '100%',
	},
	margin: {
		margin: theme.spacing.unit * 2,
	},
	padding: {
		padding: `0 ${theme.spacing.unit * 2}px`,
	},
	formControl: {
		margin: theme.spacing.unit,
		minWidth: 120,
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
	handleSelectChange = event => {
		this.setState({ [event.target.name]: event.target.value });
	};

	render() {
		const { classes } = this.props;
		const steps = getSteps();
		const { activeStep } = this.state;
		let id = 0;
		function createData(pool, description, time) {
			id += 1;
			return { id, pool, description: time + ' ' + description };
		}
		const rows = [
			createData('Pool 1', 'Archie with Ryan', '5/1/2019 1:00PM'),
			createData('Pool 2', 'Archie with Ryan', '5/1/2019 1:00PM'),
			createData('Pool 1', 'Archie with Ryan', '5/1/2019 1:00PM'),
			createData('Pool 2', 'Archie with Ryan', '5/1/2019 1:00PM'),
		];

		const { value } = this.state;
		const date = new Date();
		return (
			<div>
				<div className="row">
					<div className="col-2 d-none d-md-block"></div>
					<div className="col">
						<h3 className="display-5 text-center">Schedule your lessons</h3>
						<Badge color="primary" badgeContent={4} className={classes.margin}>
							<Typography className={classes.padding}>Available credits</Typography>
						</Badge>
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
											{date.toDateString()}
											<Tabs value={value} onChange={this.handleChange}>
												<Tab label="Pool 1" />
												<Tab label="Pool 2" />
											</Tabs>
											{value === 0 && <TabContainer>
												<Table>
													<TableHead>
														<TableRow>
															<TableCell></TableCell>
															<TableCell align="center">Time</TableCell>
															<TableCell align="center">Instructor</TableCell>
															<TableCell></TableCell>
														</TableRow>
													</TableHead>
													<TableBody>
														<TableRow align="center">
															<TableCell>
																<FormControl className={classes.formControl}>
																	<InputLabel htmlFor="age-simple">Available</InputLabel>
																	<Select
																		value={this.state.age}
																		onChange={this.handleSelectChange}
																		inputProps={{
																			name: 'age',
																			id: 'age-simple',
																		}}>
																		<MenuItem value="">
																			<em>None</em>
																		</MenuItem>
																		<MenuItem value={10}>Archie</MenuItem>
																		<MenuItem value={20}>Belle</MenuItem>
																	</Select>
																</FormControl>
															</TableCell>
															<TableCell align="center">
																1:00pm
															</TableCell>
															<TableCell align="center">Ryan</TableCell>
														</TableRow>
													</TableBody>
												</Table>

											</TabContainer>}
											{value === 1 && <TabContainer>
												<Table>
													<TableHead>
														<TableRow>
															<TableCell></TableCell>
															<TableCell align="right">Time</TableCell>
															<TableCell align="right">Instructor</TableCell>
															<TableCell></TableCell>
														</TableRow>
													</TableHead>
													<TableBody>
														<TableRow>
															<TableCell align="center">
																<FormControl className={classes.formControl}>
																	<InputLabel htmlFor="age-simple">Available</InputLabel>
																	<Select
																		value={this.state.age}
																		onChange={this.handleSelectChange}
																		inputProps={{
																			name: 'age',
																			id: 'age-simple',
																		}}>
																		<MenuItem value="">
																			<em>None</em>
																		</MenuItem>
																		<MenuItem value={10}>Archie</MenuItem>
																		<MenuItem value={20}>Belle</MenuItem>
																	</Select>
																</FormControl>
															</TableCell>
															<TableCell align="center">
																1:00pm
															</TableCell>
															<TableCell align="center">Cassandra</TableCell>
														</TableRow>
													</TableBody>
												</Table>
											</TabContainer>}
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
														<TableCell>Pool</TableCell>
														<TableCell>Lesson</TableCell>
													</TableRow>
												</TableHead>
												<TableBody>
													<TableRow>
														<TableCell scope="row">
															03/21/2019 1:00pm
													</TableCell>
														<TableCell>Pool 1</TableCell>
														<TableCell>Archie with Ryan</TableCell>
													</TableRow>
												</TableBody>
											</Table>
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
									</StepContent>
								</Step>
							</Stepper>
							{activeStep === steps.length && (
								<Paper square elevation={0} className={classes.resetContainer}>
									<Typography>All steps completed - you&apos;re finished</Typography>
									<Button onClick={this.handleReset} className={classes.button} variant="contained" color="primary">
										Schedule another lesson
							</Button>
								</Paper>
							)}
						</div>
					</div>
					<div className="col-2 d-none d-md-block"></div>
				</div>
				<div className="row">
					<div className="col">
						<br /><br /><br />
						<h3 className="display-5 text-center">Upcoming lessons</h3>
						<Table className={classes.table}>
							<TableHead>
								<TableRow>
									<TableCell>Pool</TableCell>
									<TableCell align="center">Lesson</TableCell>
									<TableCell align="center"></TableCell>
								</TableRow>
							</TableHead>
							<TableBody>
								{rows.map(row => (
									<TableRow key={row.id}>
										<TableCell style={{ minWidth: 125}}>
											<a href="#pool1" title="101 Lakeview, Stansbury Park, UT">{row.pool}</a>
										</TableCell>
										<TableCell align="center" style={{ minWidth: 250 }}>{row.description}</TableCell>
										<TableCell align="center" style={{ minWidth: 50 }}>
											<Button className={classes.button}>Cancel</Button>
										</TableCell>
									</TableRow>
								))}
							</TableBody>
						</Table>
					</div>
				</div>
			</div>
		);
	}
}

Schedule.propTypes = {
	classes: PropTypes.object,
};

export default withStyles(styles)(Schedule);