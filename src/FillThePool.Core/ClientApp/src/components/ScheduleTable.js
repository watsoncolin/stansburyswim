import React from "react";
import PropTypes from "prop-types";
import { withStyles } from "@material-ui/core/styles";
import deepOrange from "@material-ui/core/colors/deepOrange";
import deepPurple from "@material-ui/core/colors/deepPurple";
import AvailableCredits from "./AvailableCredits";
import UpcommingLessons from "./UpcommingLessons";
import * as moment from "moment";
import Table from "@material-ui/core/Table";
import TableBody from "@material-ui/core/TableBody";
import TableCell from "@material-ui/core/TableCell";
import TableContainer from "@material-ui/core/TableContainer";
import TableHead from "@material-ui/core/TableHead";
import TableRow from "@material-ui/core/TableRow";
import Paper from "@material-ui/core/Paper";
import FormControl from "@material-ui/core/FormControl";
import InputLabel from "@material-ui/core/InputLabel";
import Select from "@material-ui/core/Select";
import MenuItem from "@material-ui/core/MenuItem";
import DateFnsUtils from "@date-io/date-fns";
import { DatePicker, MuiPickersUtilsProvider } from "@material-ui/pickers";

let headers = new Headers();
headers.append("pragma", "no-cache");
headers.append("cache-control", "no-cache");

class ScheduleTable extends React.Component {
  state = {
    activeStep: 0,
    scheduleData: {
      upcommingLessons: [],
    },
    lessons: [],
    credits: 0,
    lessonSort: "time",
    sortDirection: "asc",
  };

  componentDidMount = async () => {
    await this.loadScheduleData();
    await this.loadCreditCount();
  };

  loadScheduleData = async () => {
    const response = await fetch("/api/schedule", {
      cache: "no-cache",
      method: "GET",
      credentials: "same-origin",
      headers: headers,
    });
    const scheduleData = await response.json();
    this.setState({ scheduleData });
  };

  loadCreditCount = async () => {
    const response = await fetch("/api/payments/available-credits", {
      cache: "no-cache",
      method: "GET",
      credentials: "same-origin",
      headers: headers,
    });
    const credits = await response.json();
    this.setState({ credits });
  };

  getStudents = () => {
    const { scheduleData } = this.state;

    if (scheduleData.students) {
      return scheduleData.students;
    }

    return [];
  };

  getPools = () => {
    const { scheduleData } = this.state;

    if (scheduleData.pools) {
      return scheduleData.pools;
    }

    return [];
  };

  getInstructors = () => {
    const { scheduleData } = this.state;

    if (scheduleData.lessons) {
      return scheduleData.lessons.map((l) => l.instructor);
    }

    return [];
  };

  getLessonDates = () => {
    const { scheduleData } = this.state;

    if (scheduleData.lessons) {
      return scheduleData.lessons
        .filter((l) => this.state.pool && l.poolId === this.state.pool.id)
        .map((l) => {
          return l.time;
        });
    }

    return [];
  };

  getLessonForDate = (date) => {
    const { scheduleData } = this.state;
    let dateClone = moment(date);

    if (scheduleData.lessons) {
      const lessons = scheduleData.lessons
        .filter((l) => {
          const lessonDate = moment(l.time);
          if (
            this.state.pool &&
            l.poolId === this.state.pool.id &&
            dateClone.isSame(lessonDate, "day")
          ) {
            if (l.registration === undefined) {
              l.registration = {
                student: {
                  id: -1,
                },
              };
            }
            return true;
          }
          return false;
        })
        .sort((a, b) => {
          if (this.state.lessonSort === "instructor") {
            if (a.instructor.name === b.instructor.name) {
              return 0;
            }
            return this.props.sortDirection === "asc"
              ? a.instructor.name > b.instructor.name
                ? 1
                : -1
              : a.instructor.name < b.instructor.name
              ? 1
              : -1;
          } else {
            if (a.time === b.time) {
              return 0;
            }
            return this.props.sortDirection === "asc"
              ? a.time > b.time
                ? 1
                : -1
              : a.time < b.time
              ? 1
              : -1;
          }
        });
      return lessons;
    }

    return [];
  };

  handleFinish = async () => {
    let allSuccess = true;
    for (let lesson of this.state.lessons) {
      const response = await fetch("/api/schedule/register", {
        cache: "no-cache",
        method: "POST",
        headers: { "Content-Type": "application/json", ...headers },
        credentials: "same-origin",
        body: JSON.stringify({
          studentId: lesson.registration.student.id,
          scheduleId: lesson.id,
        }),
      });

      if (response.status !== 200) {
        await this.loadScheduleData();
        allSuccess = false;
        const messages = await response.json();
        this.setState({ messages });
        break;
      }
    }

    if (allSuccess) {
      await this.loadScheduleData();
      await this.loadCreditCount();
      this.setState((state) => ({
        activeStep: state.activeStep + 1,
      }));
    }
  };

  handleReset = () => {};

  handleCancelLesson = async (registrationId) => {
    await fetch("/api/schedule/cancel/" + registrationId, {
      cache: "no-cache",
      method: "DELETE",
      credentials: "same-origin",
      headers: headers,
    });

    await this.loadScheduleData();
    await this.loadCreditCount();
  };

  handlePoolSelection = (pool) => {
    this.setState((state) => ({
      activeStep: state.activeStep + 1,
      pool,
    }));
  };
  handleDateSelection = (date) => {
    this.setState((state) => ({
      activeStep: state.activeStep + 1,
      date,
    }));
  };

  handleLessonSelection = (lesson, studentId) => {
    const { scheduleData, lessons } = this.state;
    const students = this.getStudents();

    if (lessons.length >= this.state.credits) {
      this.setState({
        messages: {
          errors: [
            {
              credits: {
                title: "Add more credits to continue registration.",
              },
            },
          ],
        },
      });
      return;
    }

    const student = students.find((s) => s.id === studentId);

    for (let i = 0; i < scheduleData.lessons.length; i++) {
      if (scheduleData.lessons[i].id === lesson.id) {
        if (student) {
          scheduleData.lessons[i].registration = {
            student,
          };
          scheduleData.lessons[i].pool = this.state.pool;
          lessons.push(scheduleData.lessons[i]);
        } else {
          scheduleData.lessons[i].registration = {
            student: {
              id: -1,
            },
          };

          const index = lessons.findIndex((val, ind) => {
            return val.id === lesson.id;
          });

          lessons.splice(index);
        }
      }
    }
    this.setState({
      lessons,
      scheduleData,
    });
  };

  handleSelectChange = (event, lesson) => {
    this.props.onSelect(lesson, event.target.value);
  };

  handleRequestSort = (event, property) => {
    const lessonSort = property;
    let sortDirection = "desc";

    if (
      this.state.lessonSort === property &&
      this.state.sortDirection === "desc"
    ) {
      sortDirection = "asc";
    }

    this.setState({ sortDirection, lessonSort });
  };

  render() {
    const { classes } = this.props;
    return (
      <div>
        <div className="row">
          <div className="col-md-2 d-none d-md-block d-lg-none"></div>
          <div className="col">
            <h3 className="display-5 text-center">Schedule your lessons</h3>
            <div className="row">
              <div className="col">
                <AvailableCredits credits={this.state.credits} />
              </div>
            </div>
            <br />
            <div className={classes.root}>
              <div>
                <FormControl className={classes.formControl}>
                  <InputLabel id="pool-helper-label">Pool</InputLabel>
                  <Select labelId="pool-helper-label">
                    <MenuItem value="">
                      <em>All</em>
                    </MenuItem>
                    {this.getPools().map((pool) => {
                      return <MenuItem value={pool.id}>{pool.name}</MenuItem>;
                    })}
                  </Select>
                </FormControl>
                <FormControl className={classes.formControl}>
                  <InputLabel id="instructor-helper-label">
                    Instructor
                  </InputLabel>
                  <Select labelId="instructor-helper-label">
                    <MenuItem value="">
                      <em>All</em>
                    </MenuItem>
                    {this.getInstructors().map((instructor) => {
                      return (
                        <MenuItem value={instructor.id}>
                          {instructor.name}
                        </MenuItem>
                      );
                    })}
                  </Select>
                </FormControl>
                <FormControl className={classes.formControl}>
                  <InputLabel id="day-of-week-helper-label">
                    Day of week
                  </InputLabel>
                  <Select labelId="day-of-week-helper-label">
                    <MenuItem value="">
                      <em>All</em>
                    </MenuItem>
                    <MenuItem value={0}>Mon</MenuItem>
                    <MenuItem value={1}>Tues</MenuItem>
                    <MenuItem value={2}>Wed</MenuItem>
                    <MenuItem value={3}>Thur</MenuItem>
                    <MenuItem value={4}>Fri</MenuItem>
                    <MenuItem value={5}>Sat</MenuItem>
                    <MenuItem value={6}>Sun</MenuItem>
                  </Select>
                </FormControl>
                <FormControl className={classes.formControl}>
                  <InputLabel id="date-helper-label">Date</InputLabel>
                  <MuiPickersUtilsProvider
                    utils={DateFnsUtils}
                    labelId="date-helper-label"
                  >
                    <div className="picker">
                      <DatePicker label="Date" defaultValue={null} />
                    </div>
                  </MuiPickersUtilsProvider>
                </FormControl>
              </div>
              <TableContainer component={Paper}>
                <Table className={classes.table} aria-label="simple table">
                  <TableHead>
                    <TableRow>
                      <TableCell>Dessert (100g serving)</TableCell>
                      <TableCell align="right">Calories</TableCell>
                      <TableCell align="right">Fat&nbsp;(g)</TableCell>
                      <TableCell align="right">Carbs&nbsp;(g)</TableCell>
                      <TableCell align="right">Protein&nbsp;(g)</TableCell>
                    </TableRow>
                  </TableHead>
                  <TableBody>
                    <TableRow>
                      <TableCell component="th" scope="row"></TableCell>
                      <TableCell align="right">cell</TableCell>
                      <TableCell align="right">cell</TableCell>
                      <TableCell align="right">cell</TableCell>
                      <TableCell align="right">cell</TableCell>
                      <TableCell align="right">cell</TableCell>
                    </TableRow>
                    <TableRow>
                      <TableCell component="th" scope="row"></TableCell>
                      <TableCell align="right">cell</TableCell>
                      <TableCell align="right">cell</TableCell>
                      <TableCell align="right">cell</TableCell>
                      <TableCell align="right">cell</TableCell>
                      <TableCell align="right">cell</TableCell>
                    </TableRow>
                    <TableRow>
                      <TableCell component="th" scope="row"></TableCell>
                      <TableCell align="right">cell</TableCell>
                      <TableCell align="right">cell</TableCell>
                      <TableCell align="right">cell</TableCell>
                      <TableCell align="right">cell</TableCell>
                      <TableCell align="right">cell</TableCell>
                    </TableRow>
                    <TableRow>
                      <TableCell component="th" scope="row"></TableCell>
                      <TableCell align="right">cell</TableCell>
                      <TableCell align="right">cell</TableCell>
                      <TableCell align="right">cell</TableCell>
                      <TableCell align="right">cell</TableCell>
                      <TableCell align="right">cell</TableCell>
                    </TableRow>
                    <TableRow>
                      <TableCell component="th" scope="row"></TableCell>
                      <TableCell align="right">cell</TableCell>
                      <TableCell align="right">cell</TableCell>
                      <TableCell align="right">cell</TableCell>
                      <TableCell align="right">cell</TableCell>
                      <TableCell align="right">cell</TableCell>
                    </TableRow>
                    <TableRow>
                      <TableCell component="th" scope="row"></TableCell>
                      <TableCell align="right">cell</TableCell>
                      <TableCell align="right">cell</TableCell>
                      <TableCell align="right">cell</TableCell>
                      <TableCell align="right">cell</TableCell>
                      <TableCell align="right">cell</TableCell>
                    </TableRow>
                  </TableBody>
                </Table>
              </TableContainer>
            </div>
          </div>
          <div className="col-md-2 d-none d-md-block d-lg-none"></div>
        </div>
        <br />
        <br />
        <br />
        <UpcommingLessons
          upcommingLessons={this.state.scheduleData.upcommingLessons}
          handleCancelLesson={this.handleCancelLesson}
        />
      </div>
    );
  }
}

ScheduleTable.propTypes = {
  classes: PropTypes.object,
};
const styles = (theme) => ({
  root: {
    width: "90%",
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
    width: "100%",
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
  avatar: {
    margin: 10,
  },
  orangeAvatar: {
    margin: 10,
    color: "#fff",
    backgroundColor: deepOrange[500],
  },
  purpleAvatar: {
    margin: 10,
    color: "#fff",
    backgroundColor: deepPurple[500],
  },
});

export default withStyles(styles)(ScheduleTable);
