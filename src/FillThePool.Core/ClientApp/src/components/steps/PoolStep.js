import 'date-fns';
import React from "react";
import { withStyles } from '@material-ui/core/styles';
import PropTypes from 'prop-types';
import Typography from '@material-ui/core/Typography';
import Button from '@material-ui/core/Button';
import Paper from '@material-ui/core/Paper';
import ButtonBase from '@material-ui/core/ButtonBase';
import DialogTitle from '@material-ui/core/DialogTitle';
import Dialog from '@material-ui/core/Dialog';



class PoolStep extends React.Component {
	state = {
		open: false,
	};

	handleClickOpenDialog = () => {
		this.setState({
			open: true,
		});
	};

	handleClose = value => {
		this.setState({ selectedValue: value, open: false });
	};

	render() {
		const { classes, pools, onSelect } = this.props;
		return (
			<div className={classes.actionsContainer}>
				<div className="row">
                    {pools.map(pool => (
						<div className="col" key={pool.id}>
							<Paper className="p-1" elevation={1}>
								<ButtonBase
									focusRipple
									key={pool.name}
									className={classes.image}
									focusVisibleClassName={classes.focusVisible}
									onClick={() => onSelect(pool)}
									style={{
										width: '100%',
									}}>
									<span
										className={classes.imageSrc}
										style={{
											backgroundImage: `url('${pool.image}')`,
										}}
									/>
									<span className={classes.imageBackdrop} />
									<span className={classes.imageButton}>
										<Typography
											component="span"
											variant="subtitle1"
											color="inherit"
											className={classes.imageTitle}>
											{pool.name}
											<span className={classes.imageMarked} />
										</Typography>
									</span>
								</ButtonBase>
								<Button color="primary" className={classes.button} onClick={this.handleClickOpenDialog}>
									Pool Details
								</Button>
							</Paper>
							<Dialog open={this.state.open} onClose={this.handleClose} aria-labelledby="simple-dialog-title">
								<DialogTitle id="simple-dialog-title">Pool Details</DialogTitle>
								<div>
                                    {pool.details}
								</div>
							</Dialog>
						</div>
					))}
				</div>
			</div>
		)
	}
}

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
	image: {
		position: 'relative',
		height: 200,
		[theme.breakpoints.down('xs')]: {
			width: '100% !important', // Overrides inline-style
			height: 100,
		},
		'&:hover, &$focusVisible': {
			zIndex: 1,
			'& $imageBackdrop': {
				opacity: 0.15,
			},
			'& $imageMarked': {
				opacity: 0,
			},
			'& $imageTitle': {
				border: '4px solid currentColor',
			},
		},
	},
	imageButton: {
		position: 'absolute',
		left: 0,
		right: 0,
		top: 0,
		bottom: 0,
		display: 'flex',
		alignItems: 'center',
		justifyContent: 'center',
		color: theme.palette.common.white,
	},
	imageSrc: {
		position: 'absolute',
		left: 0,
		right: 0,
		top: 0,
		bottom: 0,
		backgroundSize: 'cover',
		backgroundPosition: 'center 40%',
	},
	imageBackdrop: {
		position: 'absolute',
		left: 0,
		right: 0,
		top: 0,
		bottom: 0,
		backgroundColor: theme.palette.common.black,
		opacity: 0.4,
		transition: theme.transitions.create('opacity'),
	},
	imageTitle: {
		position: 'relative',
		padding: `${theme.spacing.unit * 2}px ${theme.spacing.unit * 4}px ${theme.spacing.unit + 6}px`,
	},
	imageMarked: {
		height: 3,
		width: 18,
		backgroundColor: theme.palette.common.white,
		position: 'absolute',
		bottom: -2,
		left: 'calc(50% - 9px)',
		transition: theme.transitions.create('opacity'),
	},
	focusVisible: {},
});

PoolStep.propTypes = {
	classes: PropTypes.object.isRequired,
	onSelect: PropTypes.func
	//pools: PropTypes.object.isRequired,
};

export default withStyles(styles)(PoolStep);


