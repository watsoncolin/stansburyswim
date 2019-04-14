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
import ButtonBase from '@material-ui/core/ButtonBase';
import DialogTitle from '@material-ui/core/DialogTitle';
import Dialog from '@material-ui/core/Dialog';
import ListSubheader from '@material-ui/core/ListSubheader';
import List from '@material-ui/core/List';
import ListItem from '@material-ui/core/ListItem';
import ListItemIcon from '@material-ui/core/ListItemIcon';
import ListItemText from '@material-ui/core/ListItemText';
import Avatar from '@material-ui/core/Avatar';
import deepOrange from '@material-ui/core/colors/deepOrange';
import deepPurple from '@material-ui/core/colors/deepPurple';
import Grid from '@material-ui/core/Grid';
import FormGroup from '@material-ui/core/FormGroup';
import FormControlLabel from '@material-ui/core/FormControlLabel';
import IconButton from '@material-ui/core/IconButton';
import DeleteIcon from '@material-ui/icons/Delete';
import AddShoppingCartIcon from '@material-ui/icons/AddShoppingCart';
import Fab from '@material-ui/core/Fab';
import AppBar from '@material-ui/core/AppBar';
import Toolbar from '@material-ui/core/Toolbar';

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
	image: {
		position: 'relative',
		height: 200,
		[theme.breakpoints.down('xs')]: {
			width: '100% !important', // Overrides inline-style
			height: 100,
		},
		'&:hover, &$focusVisible': {
			zIndex: 1,
			'& $imageBackdrop': {
				opacity: 0.15,
			},
			'& $imageMarked': {
				opacity: 0,
			},
			'& $imageTitle': {
				border: '4px solid currentColor',
			},
		},
	},
	focusVisible: {},
	imageButton: {
		position: 'absolute',
		left: 0,
		right: 0,
		top: 0,
		bottom: 0,
		display: 'flex',
		alignItems: 'center',
		justifyContent: 'center',
		color: theme.palette.common.white,
	},
	imageSrc: {
		position: 'absolute',
		left: 0,
		right: 0,
		top: 0,
		bottom: 0,
		backgroundSize: 'cover',
		backgroundPosition: 'center 40%',
	},
	imageBackdrop: {
		position: 'absolute',
		left: 0,
		right: 0,
		top: 0,
		bottom: 0,
		backgroundColor: theme.palette.common.black,
		opacity: 0.4,
		transition: theme.transitions.create('opacity'),
	},
	imageTitle: {
		position: 'relative',
		padding: `${theme.spacing.unit * 2}px ${theme.spacing.unit * 4}px ${theme.spacing.unit + 6}px`,
	},
	imageMarked: {
		height: 3,
		width: 18,
		backgroundColor: theme.palette.common.white,
		position: 'absolute',
		bottom: -2,
		left: 'calc(50% - 9px)',
		transition: theme.transitions.create('opacity'),
	}, avatar: {
		margin: 10,
	},
	orangeAvatar: {
		margin: 10,
		color: '#fff',
		backgroundColor: deepOrange[500],
	},
	purpleAvatar: {
		margin: 10,
		color: '#fff',
		backgroundColor: deepPurple[500],
	},
});
const images = [
	{
		url: '/da689e96-e4bb-4ac3-970a-0bae9e3261c6-GettyImages-146927186.jpg',
		title: 'Pool 1',
		width: '100%',
	},
	{
		url: '/facilities-nrfamily-webad.jpg',
		title: 'Pool 2',
		width: '100%',
	},
];
function getSteps() {
	return ['Select Pool', 'Select date', 'Pick your instructor and lesson time', 'Confirm'];
}

class Schedule extends React.Component {
	state = {
		activeStep: 0,
		value: 0,
		open: false,
		checkedA: true,
		checkedB: true,
		schedules: [],
		age: '',
		credits: 0,
	};

	componentDidMount = async() => {
		const response = await fetch('/api/payments/available-credits', {
			method: 'GET',
			credentials: 'same-origin'
		});
		const credits = await response.json();
		this.setState({ credits });
	}

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

	handleClickOpenDialog = () => {
		this.setState({
			open: true,
		});
	};

	handleClose = value => {
		this.setState({ selectedValue: value, open: false });
	};

	addSchedule = (date, instructor) => {
		let schedules = this.state.schedules;
		schedules.push({
			student: 'Archie',
			date,
			instructor,
		});
		this.setState({
			schedules
		});
	}

	getSchedule = () => {
		return this.state.schedules;
	}

	handleChange = name => event => {
		this.setState({ [name]: event.target.checked });
	};
	render() {
		console.log(this.state.age)
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
		
		const date = new Date();

		const schedules = this.getSchedule();

		return (
			<div>
				<div className="row">
					<div className="col-md-2 d-none d-md-block d-lg-none"></div>
					<div className="col">
						<h3 className="display-5 text-center">Schedule your lessons</h3>
						<h5 className="display-6 text-center">OPTION A: Better for scheduling by day.</h5>
						<div className="row">
							<div className="col">
								<Fab size="small" color="primary" aria-label="Add credits" className={classes.margin}>
									<AddShoppingCartIcon />
								</Fab>
								<Badge color="primary" badgeContent={this.state.credits} className={classes.margin}>
									<Typography className={classes.padding}>Available credits</Typography>
								</Badge>
							</div>
						</div>
						<br />
						<div className={classes.root}>
							<Stepper activeStep={activeStep} orientation="vertical">
								<Step>
									<StepLabel>Select Pool</StepLabel>
									<StepContent>
										<div className={classes.actionsContainer}>
											<div className="row">
												{images.map(image => (
													<div className="col" key={image.url}>
														<Paper className="p-1" elevation={1}>
															<ButtonBase
																focusRipple
																key={image.title}
																className={classes.image}
																focusVisibleClassName={classes.focusVisible}
																onClick={this.handleNext}
																style={{
																	width: image.width,
																}}>
																<span
																	className={classes.imageSrc}
																	style={{
																		backgroundImage: `url(${image.url})`,
																	}}
																/>
																<span className={classes.imageBackdrop} />
																<span className={classes.imageButton}>
																	<Typography
																		component="span"
																		variant="subtitle1"
																		color="inherit"
																		className={classes.imageTitle}>
																		{image.title}
																		<span className={classes.imageMarked} />
																	</Typography>
																</span>
															</ButtonBase>
															<Button color="primary" className={classes.button} onClick={this.handleClickOpenDialog}>
																Pool Details
														</Button>
														</Paper>
														<Dialog open={this.state.open} onClose={this.handleClose} aria-labelledby="simple-dialog-title">
															<DialogTitle id="simple-dialog-title">Pool Details</DialogTitle>
															<div>
																address and more details....
															</div>
														</Dialog>
													</div>
												))}
											</div>
										</div>
									</StepContent>
								</Step>
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
									<StepLabel>Pick your instructor and lesson time.</StepLabel>
									<StepContent>
										<Typography></Typography>
										<div className={classes.actionsContainer}>
											{date.toDateString()}
											<Table>
												<TableHead>
													<TableRow>
														<TableCell></TableCell>
														<TableCell align="center">Time</TableCell>
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
					<div className="col-md-2 d-none d-md-block d-lg-none"></div>
				</div>
				<div className="row">
					<div className="col-md-2 d-none d-md-block d-lg-none"></div>
					<div className="col">
						<h3 className="display-5 text-center">Schedule your lessons</h3>
						<h3 className="display-6 text-center">OPTION B: Better for scheduling one student many times.</h3>
						<Badge color="primary" badgeContent={4} className={classes.margin}>
							<Typography className={classes.padding}>Available credits</Typography>
						</Badge>
						<div className={classes.root}>
							<Stepper activeStep={activeStep} orientation="vertical">
								<Step>
									<StepLabel>Select Pool</StepLabel>
									<StepContent>
										<div className={classes.actionsContainer}>
											<div className="row">
												{images.map(image => (
													<div className="col" key={image.url}>
														<Paper className="p-1" elevation={1}>
															<ButtonBase
																focusRipple
																key={image.title}
																className={classes.image}
																focusVisibleClassName={classes.focusVisible}
																onClick={this.handleNext}
																style={{
																	width: image.width,
																}}>
																<span
																	className={classes.imageSrc}
																	style={{
																		backgroundImage: `url(${image.url})`,
																	}}
																/>
																<span className={classes.imageBackdrop} />
																<span className={classes.imageButton}>
																	<Typography
																		component="span"
																		variant="subtitle1"
																		color="inherit"
																		className={classes.imageTitle}>
																		{image.title}
																		<span className={classes.imageMarked} />
																	</Typography>
																</span>
															</ButtonBase>
															<Button color="primary" className={classes.button} onClick={this.handleClickOpenDialog}>
																Pool Details
														</Button>
														</Paper>
														<Dialog open={this.state.open} onClose={this.handleClose} aria-labelledby="simple-dialog-title">
															<DialogTitle id="simple-dialog-title">Pool Details</DialogTitle>
															<div>
																address and more details....
															</div>
														</Dialog>
													</div>
												))}
											</div>
										</div>
									</StepContent>
								</Step>
								<Step>
									<StepLabel>Select student</StepLabel>
									<StepContent>
										<div className={classes.actionsContainer}>
											<List
												component="nav"
												subheader={<ListSubheader component="div">Nested List Items</ListSubheader>}
												className={classes.root}
											>
												<ListItem button
													onClick={this.handleNext}>
													<ListItemIcon>
														<Avatar className={classes.orangeAvatar}>A</Avatar>
													</ListItemIcon>
													<ListItemText inset primary="Archie" />
												</ListItem>

												<ListItem button
													onClick={this.handleNext}>
													<ListItemIcon>
														<Avatar className={classes.purpleAvatar}>A</Avatar>
													</ListItemIcon>
													<ListItemText inset primary="Belle" />
												</ListItem>
											</List>
											<div>
											</div>
										</div>
									</StepContent>
								</Step>
								<Step>
									<StepLabel>Pick your instructor and lesson time.</StepLabel>
									<StepContent>
										<Typography>Archie</Typography>
										<div className={classes.actionsContainer}>
											<FormGroup row>
												<FormControlLabel
													control={
														<Checkbox
															checked={this.state.checkedA}
															onChange={this.handleChange('checkedA')}
															value="checkedA"
														/>
													}
													label="Ryan"
												/>
												<FormControlLabel
													control={
														<Checkbox
															checked={this.state.checkedB}
															onChange={this.handleChange('checkedB')}
															value="checkedB"
															color="primary"
														/>
													}
													label="Cassandra"
												/>
											</FormGroup>
											<FormControl className={classes.formControl}>
												<InputLabel htmlFor="age-simple">Available Dates</InputLabel>
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
													<MenuItem value={10}>5/1/2019 1:00PM - Ryan</MenuItem>
													<MenuItem value={20}>5/1/2019 2:00PM - Cassandra</MenuItem>
												</Select>
											</FormControl>

											<Button onClick={this.addSchedule} className={classes.button}>Add</Button>
											<Table>
												<TableHead>
													<TableRow>
														<TableCell align="center">Time</TableCell>
														<TableCell>Instructor</TableCell>
														<TableCell></TableCell>
													</TableRow>
												</TableHead>
												<TableBody>
													{schedules.map(() => (
														<TableRow align="center">
															<TableCell align="center">
																1:00pm
															</TableCell>
															<TableCell align="center">Ryan</TableCell>
															<TableCell align="center">
																<IconButton className={classes.button} aria-label="Delete" disabled color="primary">
																	<DeleteIcon />
																</IconButton>
															</TableCell>
														</TableRow>
													))}
												</TableBody>
											</Table>
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
					<div className="col-md-2 d-none d-md-block d-lg-none"></div>
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
										<TableCell style={{ minWidth: 125 }}>
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
			</div >
		);
	}
}

Schedule.propTypes = {
	classes: PropTypes.object,
};

export default withStyles(styles)(Schedule);