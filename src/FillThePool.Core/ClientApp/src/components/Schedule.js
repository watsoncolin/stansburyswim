import React from "react";
import PropTypes from "prop-types";
import { withStyles } from "@material-ui/core/styles";
import Typography from "@material-ui/core/Typography";
import deepOrange from "@material-ui/core/colors/deepOrange";
import deepPurple from "@material-ui/core/colors/deepPurple";
import * as moment from "moment";
import ScheduleTable from "./ScheduleTable";

let headers = new Headers();
headers.append("pragma", "no-cache");
headers.append("cache-control", "no-cache");

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

function getSteps() {
  return [
    "Select Pool",
    "Select date",
    "Pick your instructor and lesson time",
    "Confirm",
  ];
}

class Schedule extends React.Component {
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

  handleNext = () => {
    this.setState((state) => ({
      activeStep: state.activeStep + 1,
    }));
  };

  handleBack = () => {
    this.setState((state) => ({
      activeStep: state.activeStep - 1,
    }));
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

  handleReset = () => {
    const { scheduleData } = this.state;
    for (let i = 0; i < scheduleData.lessons.length; i++) {
      scheduleData.lessons[i].registration = {
        student: {
          id: -1,
        },
      };
    }
    this.setState({
      activeStep: 0,
      pool: undefined,
      date: undefined,
      lessons: [],
      scheduleData,
    });
  };

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
    return (
      <div>
        <ScheduleTable
          {...this.props}
          onSelect={this.handleLessonSelection}
        ></ScheduleTable>
        {/* <div className="row">
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
              <Stepper activeStep={activeStep} orientation="vertical">
                <Step>
                  <StepLabel>Select Pool</StepLabel>
                  <StepContent>
                    <PoolStep
                      onSelect={this.handlePoolSelection}
                      pools={this.getPools()}
                    />
                  </StepContent>
                </Step>
                <Step>
                  <StepLabel>Select date</StepLabel>
                  <StepContent>
                    <DateStep
                      onSelect={this.handleDateSelection}
                      selectedDate={this.state.date}
                      lessonDates={this.getLessonDates()}
                      {...stepProps}
                    />
                  </StepContent>
                </Step>
                <Step>
                  <StepLabel>Pick your instructor and lesson time.</StepLabel>
                  <StepContent>
                    <LessonStep
                      validationMessage={this.state.messages}
                      lessonSort={this.state.lessonSort}
                      sortDirection={this.state.sortDirection}
                      onRequestSort={this.handleRequestSort}
                      onSelect={this.handleLessonSelection}
                      selectedDate={this.state.date}
                      lessons={this.getLessonForDate(this.state.date)}
                      students={this.getStudents()}
                      {...stepProps}
                    />
                  </StepContent>
                </Step>
                <Step>
                  <StepLabel>Confirm</StepLabel>
                  <StepContent>
                    <Typography>How does this look?</Typography>
                    <ConfirmStep lessons={this.state.lessons} {...stepProps} />
                  </StepContent>
                </Step>
              </Stepper>
              {activeStep === steps.length && (
                <Paper square elevation={0} className={classes.resetContainer}>
                  <Typography>
                    All steps completed - you&apos;re finished
                  </Typography>
                  <Button
                    onClick={this.handleReset}
                    className={classes.button}
                    variant="contained"
                    color="primary"
                  >
                    Schedule another lesson
                  </Button>
                </Paper>
              )}
            </div>
          </div>
          <div className="col-md-2 d-none d-md-block d-lg-none"></div>
        </div>
        <br />
        <br />
        <br /> */}
        {/* <UpcommingLessons
          upcommingLessons={this.state.scheduleData.upcommingLessons}
          handleCancelLesson={this.handleCancelLesson}
        /> */}
      </div>
    );
  }
}

Schedule.propTypes = {
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
});

export default withStyles(styles)(Schedule);
