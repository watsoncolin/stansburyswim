import 'date-fns';
import React from "react";
import { withStyles } from '@material-ui/core/styles';
import PropTypes from 'prop-types';
import Table from '@material-ui/core/Table';
import TableBody from '@material-ui/core/TableBody';
import TableCell from '@material-ui/core/TableCell';
import TableHead from '@material-ui/core/TableHead';
import TableRow from '@material-ui/core/TableRow';
import StepButtons from './StepButtons';
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
});


class ConfirmStep extends React.Component {
	state = {
		student: '',
	};
	render() {
        const { classes, lessons } = this.props;
		return (
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
                        {lessons.map((lesson) => (
                            <TableRow key={lesson.id}>
                                <TableCell scope="row">{moment(lesson.time).format('LLL')}</TableCell>
                                <TableCell>{lesson.pool.name}</TableCell>
                                <TableCell>{lesson.registration.student.name} with {lesson.instructor.name}</TableCell>
                            </TableRow>
                        ))}						
					</TableBody>
				</Table>
				<div>
					<br />
					<br />
					<StepButtons {...this.props} />
				</div>
			</div>
		)
	}
}

ConfirmStep.propTypes = {
	classes: PropTypes.object.isRequired,
    lessons: PropTypes.array.isRequired,
	onSelect: PropTypes.func,
	handleBack: PropTypes.func,
	handleNext: PropTypes.func,
	//pools: PropTypes.object.isRequired,
};

export default withStyles(styles)(ConfirmStep);