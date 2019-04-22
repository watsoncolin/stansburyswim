import 'date-fns';
import React from "react";
import { withStyles } from '@material-ui/core/styles';
import PropTypes from 'prop-types';
import Typography from '@material-ui/core/Typography';
import TextField from '@material-ui/core/TextField';
import Calendar from './Calendar';
import * as moment from 'moment';
import Select from '@material-ui/core/Select';
import MenuItem from '@material-ui/core/MenuItem';
import FormControl from '@material-ui/core/FormControl';
import InputLabel from '@material-ui/core/InputLabel';
import Table from '@material-ui/core/Table';
import TableBody from '@material-ui/core/TableBody';
import TableCell from '@material-ui/core/TableCell';
import TableHead from '@material-ui/core/TableHead';
import TableRow from '@material-ui/core/TableRow';
import Button from '@material-ui/core/Button';


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
	textField: {
		marginLeft: theme.spacing.unit,
		marginRight: theme.spacing.unit,
		width: 200,
	},
});


class AdminSchedule extends React.Component {
	state = {
		date: moment(),
		start: moment().minute(0),
		end: moment().minute(0).add(20, 'm'),
		pools: [],
		pool: {
			id: -1
		},
		instructors: [],
		instructor: {
			id: -1
		},
		messages: { errors: [] },
		schedules: []
	};

	componentDidMount = async () => {
		await this.loadSchedule(this.state.date);
		await this.loadPools();
		await this.loadInstructors();
	}

	loadSchedule = async (date) => {
		const response = await fetch('/api/admin/schedule/' + moment(date).format('YYYY-MM-DD'), {
			method: 'GET',
			credentials: 'same-origin'
		});
		const schedules = await response.json();
		this.setState({ schedules });
	}

	loadPools = async () => {
		const response = await fetch('/api/admin/schedule/pools', {
			method: 'GET',
			credentials: 'same-origin'
		});
		const pools = await response.json();
		this.setState({ pools });
	}

	loadInstructors = async () => {
		const response = await fetch('/api/admin/schedule/instructors', {
			method: 'GET',
			credentials: 'same-origin'
		});
		const instructors = await response.json();
		this.setState({ instructors });
	}

	handlePoolChange = (event) => {
		const pool = this.state.pools.find((p) => p.id === event.target.value);

		this.setState({
			pool
		});
	}

	handleInstructorChange = (event) => {
		const instructor = this.state.instructors.find((i) => i.id === event.target.value);

		this.setState({
			instructor
		});
	}

	onSelect = (date) => {
		const start = moment(date);
		start.hour(11)
		start.minute(0)
		const end = moment(start);
		end.add(20, 'm');
		this.setState({ date: start, start, end })
		this.loadSchedule(date);
	}

	onStartChange = (event) => {
		const start = moment(`${this.state.date.format('YYYY-MM-DD')} ${event.target.value}`)
		this.setState({
			start,
			end: moment(start).add(20, 'm')
		})
	}

	onDelete = async (scheduleId) => {
		await fetch('/api/admin/schedule/' + scheduleId,  {
			method: 'DELETE',
			credentials: 'same-origin'
		});

		await this.loadSchedule(this.state.date);
	}

	addSchedule = async () => {

		const pool = this.state.pool.id > 0 ? this.state.pool : undefined;
		const instructor = this.state.instructor.id > 0 ? this.state.instructor : undefined;

		const response = await fetch('/api/admin/schedule/', {
			method: 'POST',
			headers: { 'Content-Type': 'application/json' },
			credentials: 'same-origin',
			body: JSON.stringify({
				start: this.state.start,
				end: this.state.end,
				pool,
				instructor,
			})
		});
		if (response.status === 200) {
			this.setState({
				start: moment(this.state.start).add(20, 'm'),
				end: moment(this.state.start).add(40, 'm'),
				messages: { errors: [] },
			});
			await this.loadSchedule(this.state.date);
		}
		else {
			const messages = await response.json();
			this.setState({ messages });
		}
	}

	render() {
		const { classes } = this.props;
		return (
			<div className={classes.actionsContainer}>
				<Typography>Select a day to schedule lessons.</Typography>
				<Calendar selectedDate={this.state.date} onSelect={this.onSelect} lessonDates={[]} />
				<Table className={classes.table}>
					<TableHead>
						<TableRow>
							<TableCell>Pool</TableCell>
							<TableCell align="center">Lesson</TableCell>
							<TableCell align="center"></TableCell>
						</TableRow>
					</TableHead>
					<TableBody>
						{this.state.schedules.map(schedule => (
							<TableRow key={schedule.id}>
								<TableCell style={{ minWidth: 125 }}>
									{schedule.pool.name}
								</TableCell>
								<TableCell align="center" style={{ minWidth: 250 }}>{moment(schedule.start).format('LT')} - {moment(schedule.end).format('LT')} with {schedule.instructor.name}</TableCell>
								<TableCell align="center" style={{ minWidth: 50 }}>
									<Button className={classes.button} onClick={(e) => this.onDelete(schedule.id)}>Delete</Button>
								</TableCell>
							</TableRow>
						))}
					</TableBody>
				</Table>
				<br />
				<br />
				<br />
				<div>
					<h5 className="display-5 text-center">Add Schedule</h5>
					<div className={classes.container}>
						<TextField
							id="start"
							label="Start"
							type="time"
							value={this.state.start.format('HH:mm')}
							className={classes.textField}
							InputLabelProps={{
								shrink: true,
							}}
							inputProps={{
								step: 1200, // 20 min
							}}
							onChange={(e) => this.onStartChange(e)}
						/>
						<TextField
							disabled
							id="end"
							label="End"
							type="time"
							value={this.state.end.format('HH:mm')}
							className={classes.textField}
							InputLabelProps={{
								shrink: true,
							}}
							inputProps={{
								step: 1200, // 20 min
							}}
						/>

						<FormControl className={classes.formControl}>
							<InputLabel htmlFor="pool-simple">Pool</InputLabel>
							<Select
								error={this.state.messages.errors['Pool'] !== undefined}
								value={this.state.pool.id}
								onChange={this.handlePoolChange}
								inputProps={{
									name: 'pool',
									id: 'pool-simple',
								}}>
								<MenuItem value="-1">
									<em>None</em>
								</MenuItem>
								{this.state.pools.map((pool) => (
									<MenuItem key={pool.name} value={pool.id}>{pool.name}</MenuItem>
								))}
							</Select>
						</FormControl>
						<FormControl className={classes.formControl}>
							<InputLabel htmlFor="instructor-simple">Instructor</InputLabel>
							<Select
								error={this.state.messages.errors['Instructor'] !== undefined}
								value={this.state.instructor.id}
								onChange={this.handleInstructorChange}
								inputProps={{
									name: 'instructor',
									id: 'instructor-simple',
								}}>
								<MenuItem value="-1">
									<em>None</em>
								</MenuItem>
								{this.state.instructors.map((instructor) => (
									<MenuItem key={instructor.name} value={instructor.id}>{instructor.name}</MenuItem>
								))}
							</Select>
						</FormControl>
					</div>
					<br />
					<button type="button" className="btn btn-primary" onClick={this.addSchedule}>Add</button>
				</div>
			</div>
		)
	}
}

AdminSchedule.propTypes = {
	classes: PropTypes.object.isRequired,
};

export default withStyles(styles)(AdminSchedule);