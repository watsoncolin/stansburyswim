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
});


class UpcommingLessons extends React.Component {
	state = {
		student: '',
	};

	cancelLesson = (lessonId) => {
		this.props.handleCancelLesson(lessonId);
	}

	render() {
		const { classes, upcommingLessons } = this.props;
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
										<a href="#pool1" title={lesson.pool.address}>{lesson.pool.name}</a>
									</TableCell>
									<TableCell align="center" style={{ minWidth: 250 }}>{moment(lesson.start).format('LT')} {lesson.student.name} with {lesson.instructor.name} </TableCell>
									<TableCell align="center" style={{ minWidth: 50 }}>
										<Button className={classes.button} onClick={(e) => this.cancelLesson(lesson.id)}>Cancel</Button>
									</TableCell>
								</TableRow>
							))}
						</TableBody>
					</Table>
				</div>
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