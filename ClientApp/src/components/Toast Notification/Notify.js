import React, { Component } from 'react';

export class Notify extends Component {
    displayName = Notify.name

    render() {
        return (
            <div className="toast-notification">
                <div className="icon">
                </div>
                <div className="message">
                </div>
            </div>
            )
    }
}