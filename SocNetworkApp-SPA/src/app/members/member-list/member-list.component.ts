import { Pagination, PaginatedResult } from './../../_models/pagination';
import { ActivatedRoute } from '@angular/router';
import { AlertifyService } from './../../_services/alertify.service';
import { Component, OnInit } from '@angular/core';
import { UserService } from '../../_services/user.service';
import { User } from '../../_models/user';

@Component({
    selector: 'app-member-list',
    templateUrl: './member-list.component.html',
    styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit {
    users: User[];
    user: User = JSON.parse(localStorage.getItem('user'));
    genderList = [{ value: 'male', display: 'Males' }, { value: 'female', display: 'Females' }, { value: 'all', display: 'All' }];
    userParams: any = {};
    pagination: Pagination;

    constructor(private userService: UserService,
        private alertify: AlertifyService,
        private route: ActivatedRoute) { }

    ngOnInit() {
        this.route.data.subscribe(data => {
            this.users = data['users'].result;
            this.pagination = data['users'].pagination;
        });

        this.setDefaultFilters();
    }

    pageChanged(event: any): void {
        this.pagination.currentPage = event.page;
        this.loadUsers();
    }

    resetFilters() {
        this.setDefaultFilters();
        this.loadUsers();
    }

    setDefaultFilters() {
        this.userParams.gender = 'all';
        this.userParams.minAge = 18;
        this.userParams.maxAge = 99;
        this.userParams.orderBy = 'lastActive';
    }

    loadUsers() {
        this.userService.getUsers(this.pagination.currentPage, this.pagination.itemsPerPage, this.userParams)
            .subscribe((paginationResult: PaginatedResult<User[]>) => {
                this.users = paginationResult.result;
                this.pagination = paginationResult.pagination;
            }, error => {
                this.alertify.error(error);
            });
    }
}
