import 'date-fns';
import React from "react";
import { withStyles } from '@material-ui/core/styles';
import PropTypes from 'prop-types';
import Table from '@material-ui/core/Table';
import TableBody from '@material-ui/core/TableBody';
import TableCell from '@material-ui/core/TableCell';
import TableHead from '@material-ui/core/TableHead';
import TableRow from '@material-ui/core/TableRow';
import Button from '@material-ui/core/Button';
import DialogTitle from '@material-ui/core/DialogTitle';
import Dialog from '@material-ui/core/Dialog';
import * as moment from 'moment';

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
	table: {
		width: '100%',
	},
	dialogDetails: {
		padding: `0 ${theme.spacing.unit * 2}px`,
		margin: theme.spacing.unit * 2,
	},
});


class UpcommingLessons extends React.Component {
	state = {
		student: '',
		open: false,
	};

	cancelLesson = (lessonId) => {
		this.props.handleCancelLesson(lessonId);
	}

	handleClickOpenDialog = (poolId) => {
		this.setState({
			open: poolId,
		});
	};

	handleClose = value => {
		this.setState({ selectedValue: value, open: false });
	};

	render() {
		const { classes, upcommingLessons } = this.props;
		const pools = [...new Set(upcommingLessons.map((l) => l.pool))];
		return (
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
							{upcommingLessons.map(lesson => (
								<TableRow key={lesson.id}>
									<TableCell style={{ minWidth: 125 }}>
									<Button color="primary" className={classes.button} onClick={() => this.handleClickOpenDialog(lesson.pool.id)}>
										{lesson.pool.name}
									</Button>
									</TableCell>
									<TableCell align="center" style={{ minWidth: 250 }}>{moment(lesson.lessonTime).format('LT')} {lesson.student.name} with {lesson.instructor.name} </TableCell>
									<TableCell align="center" style={{ minWidth: 50 }}>
										<Button className={classes.button} onClick={(e) => this.cancelLesson(lesson.id)}>Cancel</Button>
									</TableCell>
								</TableRow>
							))}
						</TableBody>
					</Table>
				</div>
				{pools.map(pool => (
					<Dialog open={this.state.open === pool.id} onClose={this.handleClose} aria-labelledby="simple-dialog-title" key={pool.id}>
						<DialogTitle id="simple-dialog-title">Pool Details</DialogTitle>
						<div className={classes.dialogDetails}>
							{pool.details}
						</div>
					</Dialog>
				))}
			</div>
		)
	}
}

UpcommingLessons.propTypes = {
	classes: PropTypes.object.isRequired,
	onSelect: PropTypes.func,
	handleCancelLesson: PropTypes.func,
	upcommingLessons: PropTypes.array,
};

export default withStyles(styles)(UpcommingLessons);