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
import DialogContent from '@material-ui/core/DialogContent';
import DialogContentText from '@material-ui/core/DialogContentText';
import Typography from '@material-ui/core/Typography';
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
	image: {
		width: '100%'
	}
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

	renderCancelButton = lesson => {
		if (lesson.canCancel) {
			return (<Button className={this.props.classes.button} onClick={() => this.cancelLesson(lesson.id)}>Cancel</Button>);
		}

		return <span />;
	}

	render() {
		const { classes, upcommingLessons } = this.props;
		const pools = upcommingLessons.map((l) => l.pool);
		const filteredPools = {};
		for (let pool of pools) {
			if (filteredPools[pool.id] === undefined) {
				filteredPools[pool.id] = pool;
			}
		}
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
										{this.renderCancelButton(lesson)}
									</TableCell>
								</TableRow>
							))}
						</TableBody>
					</Table>
				</div>
				{Object.keys(filteredPools).map((key) => {
					const pool = filteredPools[key];
					return (
						<Dialog open={this.state.open === pool.id} onClose={this.handleClose} aria-labelledby="simple-dialog-title" key={pool.id}>
							<DialogTitle id="simple-dialog-title">{pool.name}</DialogTitle>
							<DialogContent>
								<Typography variant="subtitle1" gutterBottom>
									{pool.address}
								</Typography>
								<div>
									<img src={pool.image} alt={pool.name} className={classes.image} />
								</div>
								<DialogContentText>
									{pool.details}
								</DialogContentText>
							</DialogContent>
						</Dialog>
					)
				})}
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