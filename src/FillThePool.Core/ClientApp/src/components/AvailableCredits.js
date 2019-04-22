import 'date-fns';
import React from "react";
import { withStyles } from '@material-ui/core/styles';
import Badge from '@material-ui/core/Badge';
import PropTypes from 'prop-types';
import Typography from '@material-ui/core/Typography';

const styles = theme => ({
    margin: {
        margin: theme.spacing.unit * 2,
    },
    padding: {
        padding: `0 ${theme.spacing.unit * 2}px`,
    },
});


class AvailableCredits extends React.Component {
	render() {
        const { classes, credits } = this.props;
        let AvailableCredits = (<Badge color="primary" badgeContent={credits} className={classes.margin}>
            <Typography className={classes.padding}>Available credits</Typography>
        </Badge>);

        if (credits === 0) {
            AvailableCredits = <a href="/Pricing">Click here to add credits.</a>
        }

        return (
            <span>
                {AvailableCredits}
            </span>
        )
    }
}

AvailableCredits.propTypes = {
    classes: PropTypes.object.isRequired,
    credits: PropTypes.number.isRequired,
};

export default withStyles(styles)(AvailableCredits);