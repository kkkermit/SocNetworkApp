import { AlertifyService } from './../_services/alertify.service';
import { ActivatedRoute } from '@angular/router';
import { AuthService } from 'src/app/_services/auth.service';
import { UserService } from './../_services/user.service';
import { Pagination } from './../_models/pagination';
import { Message } from './../_models/message';
import { Component, OnInit } from '@angular/core';

@Component({
    selector: 'app-messages',
    templateUrl: './messages.component.html',
    styleUrls: ['./messages.component.css']
})
export class MessagesComponent implements OnInit {
    messages: Message[];
    pagination: Pagination;
    messageContainer = 'Unread';

    constructor(private userService: UserService,
        private authService: AuthService,
        private route: ActivatedRoute,
        private alertify: AlertifyService) { }

    ngOnInit() {
        this.route.data.subscribe(data => {
            this.messages = data['messages'].result;
            this.pagination = data['messages'].pagination;
        });
    }

    deleteMessage(id: string) {
        this.alertify.confirm('Delete message', 'Are you sure you want to delete this message?', () => {
            this.userService.deleteMessage(id, this.authService.decodedToken.nameid).subscribe(() => {
                this.messages.splice(this.messages.findIndex(m => m.id === id), 1);
                this.alertify.success('Message has been deleted');
            }, _ => {
                this.alertify.error('Failed to delete the message');
            });
        }, () => { });
    }

    loadMessages() {
        this.userService.getMessages(this.authService.decodedToken.nameid, this.pagination.currentPage,
            this.pagination.itemsPerPage, this.messageContainer).subscribe(pagedList => {
                this.messages = pagedList.result;
                this.pagination = pagedList.pagination;
            }, error => {
                this.alertify.error(error);
            });
    }

    pageChanged(event: any) {
        this.pagination.currentPage = event.page;
        this.loadMessages();
    }




}
