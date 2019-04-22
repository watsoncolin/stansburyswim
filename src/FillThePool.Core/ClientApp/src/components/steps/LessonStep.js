import 'date-fns';
import React from "react";
import { withStyles } from '@material-ui/core/styles';
import PropTypes from 'prop-types';
import MenuItem from '@material-ui/core/MenuItem';
import Table from '@material-ui/core/Table';
import TableBody from '@material-ui/core/TableBody';
import TableCell from '@material-ui/core/TableCell';
import TableHead from '@material-ui/core/TableHead';
import TableRow from '@material-ui/core/TableRow';
import FormControl from '@material-ui/core/FormControl';
import Select from '@material-ui/core/Select';
import InputLabel from '@material-ui/core/InputLabel';
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


class LessonStep extends React.Component {
    state = {
        student: '',
    };

    componentDidMount = () => {
        if (this.props.selectedDate === undefined) {
            this.props.handleBack();
        }
    }

	handleSelectChange = (event, lesson) => {
		this.props.onSelect(lesson, event.target.value);
    };

    render() {
        const { classes, selectedDate, lessons, students } = this.props;
        let selectedDateFormatted = '';
        if (selectedDate) {
            selectedDateFormatted = selectedDate.toDateString();
		}

		let warning = <span />

		if (this.props.validationMessage && this.props.validationMessage.errors) {
			debugger;
			for (let error of this.props.validationMessage.errors) {
				if (error.credits) {
					warning = (
						<div className="alert alert-warning" role="alert">
							{error.credits.title}
						</div>
					)
				}
			}			
		}


        return (
            <div className={classes.actionsContainer}>
				{selectedDateFormatted}
				{warning}
                <Table>
                    <TableHead>
                        <TableRow>
                            <TableCell>Student</TableCell>
                            <TableCell align="center">Time</TableCell>
                            <TableCell align="center">Instructor</TableCell>
                        </TableRow>
                    </TableHead>
                    <TableBody>
                        {lessons.map((lesson) => (
                            <TableRow align="center" key={lesson.time}>
                                <TableCell>
                                    <FormControl className={classes.formControl}>
                                        <InputLabel htmlFor="age-simple">Available</InputLabel>
                                        <Select
                                            value={lesson.registration.student.id}
                                            onChange={(e) => this.handleSelectChange(e, lesson)}
                                            inputProps={{
                                                name: 'student',
                                                id: 'student-simple',
                                            }}>
                                            <MenuItem value="-1">
                                                <em>None</em>
                                            </MenuItem>
                                            {students.map((student) => (
                                                <MenuItem key={student.name} value={student.id}>{student.name}</MenuItem>
											))}
                                        </Select>
                                    </FormControl>
                                </TableCell>
                                <TableCell align="center">{moment(lesson.time).format('LT')}</TableCell>
                                <TableCell align="center">{lesson.instructor.name}</TableCell>
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

LessonStep.propTypes = {
    classes: PropTypes.object.isRequired,
    onSelect: PropTypes.func,
    handleBack: PropTypes.func,
    handleNext: PropTypes.func,
    //pools: PropTypes.object.isRequired,
};

export default withStyles(styles)(LessonStep);