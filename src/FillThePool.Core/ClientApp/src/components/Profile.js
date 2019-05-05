import React from 'react';
import { withStyles } from '@material-ui/core/styles';
import UpcommingLessons from './UpcommingLessons';

let headers = new Headers();
headers.append('pragma', 'no-cache');
headers.append('cache-control', 'no-cache');

class Schedule extends React.Component {
	state = {
		scheduleData: {
			upcommingLessons: []
		},
	};

	componentDidMount = async () => {
		await this.loadScheduleData();
	}

	loadScheduleData = async () => {
		const response = await fetch('/api/schedule', {
			cache: 'no-cache',
			method: 'GET',
			credentials: 'same-origin',
			headers: headers,
		});
		const scheduleData = await response.json();
		this.setState({ scheduleData });
	}
	
	handleCancelLesson = async registrationId => {
		await fetch('/api/schedule/cancel/' + registrationId, {
			cache: 'no-cache',
			method: 'DELETE',
			credentials: 'same-origin',
			headers: headers,
		});

		await this.loadScheduleData();
	};


	render() {
		return (
			<UpcommingLessons upcommingLessons={this.state.scheduleData.upcommingLessons} handleCancelLesson={this.handleCancelLesson} />
		);
	}
}

const styles = theme => ({});
export default withStyles(styles)(Schedule);