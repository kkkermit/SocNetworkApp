import { AlertifyService } from './../../_services/alertify.service';
import { AuthService } from 'src/app/_services/auth.service';
import { UserService } from 'src/app/_services/user.service';
import { Message } from './../../_models/message';
import { Component, OnInit, Input } from '@angular/core';
import { tap } from 'rxjs/operators';

@Component({
    selector: 'app-member-messages',
    templateUrl: './member-messages.component.html',
    styleUrls: ['./member-messages.component.css']
})
export class MemberMessagesComponent implements OnInit {
    @Input()
    recipientId: string;
    messages: Message[];
    newMessage: any = {};

    constructor(private userService: UserService,
        private authSerice: AuthService,
        private alertify: AlertifyService) { }

    ngOnInit() {
        this.loadMessages();
    }

    loadMessages() {
        const currentUserId = this.authSerice.decodedToken.nameid;

        this.userService.getMessageThread(this.authSerice.decodedToken.nameid, this.recipientId)
            .pipe(tap(messages => {
                for (let i = 0; i < messages.length; i++) {
                    const message = messages[i];
                    if (!message.isRead && message.recipientId === currentUserId) {
                        this.userService.markAsRead(currentUserId, message.id);
                    }
                }
            }))
            .subscribe(messages => {
                this.messages = messages;
            }, error => {
                this.alertify.error(error);
            });
    }

    sendMessage() {
        this.newMessage.recipientId = this.recipientId;
        this.userService.sendMessage(this.authSerice.decodedToken.nameid, this.newMessage)
            .subscribe((message: Message) => {
                this.messages.unshift(message);
                this.newMessage.content = '';
            }, error => {
                this.alertify.error(error);
            });
    }
}
