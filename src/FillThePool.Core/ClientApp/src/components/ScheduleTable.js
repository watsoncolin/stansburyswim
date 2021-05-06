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
import Grid from "@material-ui/core/Grid";
import DateFnsUtils from "@date-io/date-fns";
import { DatePicker, MuiPickersUtilsProvider } from "@material-ui/pickers";
import { TableFooter, Typography, Button } from "@material-ui/core";
import DialogTitle from "@material-ui/core/DialogTitle";
import Dialog from "@material-ui/core/Dialog";
import DialogContent from "@material-ui/core/DialogContent";
import DialogContentText from "@material-ui/core/DialogContentText";

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
    pool: {},
    instructor: {},
    dayOfWeek: null,
    date: null,
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
      return Array.from(
        new Set(scheduleData.lessons.map((l) => l.instructor).map((a) => a.id))
      ).map((id) => {
        return scheduleData.lessons
          .map((l) => l.instructor)
          .find((a) => a.id === id);
      });
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

  filterPools = (lesson) => {
    return (
      lesson.poolId === this.state.pool.id || this.state.pool.id === undefined
    );
  };

  filterInstructor = (lesson) => {
    return (
      lesson.instructor.id === this.state.instructor.id ||
      this.state.instructor.id === undefined
    );
  };

  filterDate = (lesson) => {
    let time = moment(lesson.time);
    return this.state.date === null || time.isSame(this.state.date, "day");
  };

  filterDayOfWeek = (lesson) => {
    let time = moment(lesson.time);
    return this.state.dayOfWeek === null || time.day() === this.state.dayOfWeek;
  };

  getFilteredLessons = () => {
    const { scheduleData } = this.state;

    if (scheduleData.lessons) {
      const lessons = scheduleData.lessons
        .filter(this.filterPools)
        .filter(this.filterInstructor)
        .filter(this.filterDayOfWeek)
        .filter(this.filterDate);

      return lessons.map((lesson) => {
        if (!lesson.registration) {
          lesson.registration = {
            student: {},
          };
        }

        lesson.pool = this.getPools().find((p) => p.id === lesson.poolId);

        return lesson;
      });
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

  handlePoolChange = (evt) => {
    const pool = this.getPools().find((p) => p.id === evt.target.value);
    this.setState({ pool: pool ?? {} });
  };

  handleInstructorChange = (evt) => {
    const instructor = this.getInstructors().find(
      (p) => p.id === evt.target.value
    );
    this.setState({ instructor: instructor ?? {} });
  };

  handleDayOfWeekChange = (evt) => {
    const dayOfWeek = evt.target.value;
    this.setState({ dayOfWeek, date: null });
  };

  handleDateChange = (date) => {
    this.setState({ date, dayOfWeek: null });
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
      this.setState({
        lessons: [],
      });
    }
  };

  handleClickOpenDialog = (poolId) => {
    this.setState({
      open: poolId,
    });
  };

  handleClose = (value) => {
    this.setState({ selectedValue: value, open: false });
  };

  renderCancelButton = (lesson) => {
    if (lesson.canCancel) {
      return (
        <Button
          className={this.props.classes.button}
          onClick={() => this.cancelLesson(lesson.id)}
        >
          Cancel
        </Button>
      );
    }

    return <span />;
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

  render() {
    const { classes } = this.props;
    return (
      <div>
        <h3 className="display-5 text-center">Schedule your lessons</h3>
        <AvailableCredits credits={this.state.credits} />
        <br />
        <div className={classes.root}>
          <Grid container spacing={2}>
            <Grid item md={12}>
              <div>
                <FormControl className={classes.formControl}>
                  <InputLabel id="pool-helper-label">Pool</InputLabel>
                  <Select
                    labelId="pool-helper-label"
                    value={this.state.pool.id}
                    onChange={this.handlePoolChange}
                    size="small"
                  >
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
                  <Select
                    labelId="instructor-helper-label"
                    value={this.state.instructor.id}
                    onChange={this.handleInstructorChange}
                    size="small"
                  >
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
                  <InputLabel id="day-of-week-helper-label">Day</InputLabel>
                  <Select
                    labelId="day-of-week-helper-label"
                    value={this.state.dayOfWeek}
                    onChange={this.handleDayOfWeekChange}
                  >
                    <MenuItem value={null}></MenuItem>
                    <MenuItem value={1}>Monday</MenuItem>
                    <MenuItem value={2}>Tuesday</MenuItem>
                    <MenuItem value={3}>Wednesday</MenuItem>
                    <MenuItem value={4}>Thursday</MenuItem>
                    <MenuItem value={5}>Friday</MenuItem>
                    <MenuItem value={6}>Saturday</MenuItem>
                    <MenuItem value={0}>Sunday</MenuItem>
                  </Select>
                </FormControl>
                <FormControl className={classes.formControl}>
                  <MuiPickersUtilsProvider
                    utils={DateFnsUtils}
                    labelId="date-helper-label"
                    margin=""
                  >
                    <DatePicker
                      label="Date"
                      clearable={true}
                      value={this.state.date}
                      onChange={this.handleDateChange}
                      disablePast={true}
                      autoOk={true}
                    />
                  </MuiPickersUtilsProvider>
                </FormControl>
              </div>
              <TableContainer component={Paper}>
                <Table className={classes.table} size="small">
                  <TableHead>
                    <TableRow>
                      <TableCell>Student</TableCell>
                      <TableCell align="right">Date and Time</TableCell>
                      <TableCell align="right">Instructor</TableCell>
                      <TableCell align="right">
                        Pool
                        <Button
                          style={{ margin: "15px" }}
                          variant="contained"
                          color="primary"
                          onClick={this.handleFinish}
                          className={classes.button}
                          disabled={
                            this.state.lessons == null ||
                            this.state.lessons.length === 0
                          }
                        >
                          Schedule {this.state.lessons.length} lessons
                        </Button>
                      </TableCell>
                    </TableRow>
                  </TableHead>
                  <TableBody>
                    {this.getFilteredLessons().map((lesson) => {
                      return (
                        <TableRow>
                          <TableCell>
                            <FormControl className={classes.formControl}>
                              <InputLabel htmlFor="age-simple">
                                Available
                              </InputLabel>
                              <Select
                                value={lesson.registration.student.id ?? -1}
                                onChange={(e) =>
                                  this.handleLessonSelection(
                                    lesson,
                                    e.target.value
                                  )
                                }
                                inputProps={{
                                  name: "student",
                                  id: "student-simple",
                                }}
                              >
                                <MenuItem value={-1}>
                                  <em>Select</em>
                                </MenuItem>
                                {this.getStudents().map((student) => (
                                  <MenuItem
                                    key={student.name}
                                    value={student.id}
                                  >
                                    {student.name}
                                  </MenuItem>
                                ))}
                              </Select>
                            </FormControl>
                          </TableCell>
                          <TableCell align="right">
                            {moment(lesson.time).format("lll")}
                          </TableCell>
                          <TableCell align="right">
                            <Button
                              color="primary"
                              className={classes.button}
                              onClick={() =>
                                this.handleClickOpenDialog(lesson.instructor.id)
                              }
                            >
                              {lesson.instructor.name}
                            </Button>
                          </TableCell>
                          <TableCell align="right">
                            <Button
                              color="primary"
                              className={classes.button}
                              onClick={() =>
                                this.handleClickOpenDialog(lesson.pool.id)
                              }
                            >
                              {lesson.pool.name}
                            </Button>
                          </TableCell>
                        </TableRow>
                      );
                    })}
                  </TableBody>
                  <TableFooter className={classes.padding}>
                    <Button
                      style={{ margin: "15px" }}
                      variant="contained"
                      color="primary"
                      onClick={this.handleFinish}
                      className={classes.button}
                      disabled={
                        this.state.lessons == null ||
                        this.state.lessons.length === 0
                      }
                    >
                      Schedule {this.state.lessons.length} lessons
                    </Button>
                    <br />
                  </TableFooter>
                </Table>
              </TableContainer>
            </Grid>
          </Grid>
        </div>
        <br />
        <br />
        <br />
        <UpcommingLessons
          upcommingLessons={this.state.scheduleData.upcommingLessons}
          handleCancelLesson={this.handleCancelLesson}
        />
        {Object.keys(this.getPools()).map((key) => {
          const pool = this.getPools()[key];
          return (
            <Dialog
              open={this.state.open === pool.id}
              onClose={this.handleClose}
              aria-labelledby="pool-dialog-title"
              key={pool.id}
            >
              <DialogTitle id="pool-dialog-title">{pool.name}</DialogTitle>
              <DialogContent>
                <Typography gutterBottom>{pool.address}</Typography>
                <div>
                  <img
                    src={pool.image}
                    alt={pool.name}
                    className={classes.image}
                  />
                </div>
                <DialogContentText>{pool.details}</DialogContentText>
              </DialogContent>
            </Dialog>
          );
        })}
        {Object.keys(this.getInstructors()).map((key) => {
          const instructor = this.getInstructors()[key];
          return (
            <Dialog
              open={this.state.open === instructor.id}
              onClose={this.handleClose}
              aria-labelledby="instructor-dialog-title"
              key={instructor.id}
            >
              <DialogTitle id="instructor-dialog-title">
                {instructor.name}
              </DialogTitle>
              <DialogContent>
                <Typography gutterBottom>{instructor.address}</Typography>
                <div>
                  <img
                    src={instructor.image}
                    alt={instructor.name}
                    className={classes.image}
                  />
                </div>
                <DialogContentText>{instructor.bio}</DialogContentText>
              </DialogContent>
            </Dialog>
          );
        })}
      </div>
    );
  }
}

ScheduleTable.propTypes = {
  classes: PropTypes.object,
};
const styles = (theme) => ({
  root: {},
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
    minWidth: 90,
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
  image: {
    width: "100%",
  },
});

export default withStyles(styles)(ScheduleTable);
