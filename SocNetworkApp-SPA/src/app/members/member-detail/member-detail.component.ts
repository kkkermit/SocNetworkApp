import { AuthService } from 'src/app/_services/auth.service';
import { AlertifyService } from './../../_services/alertify.service';
import { UserService } from './../../_services/user.service';
import { Component, OnInit, ViewChild } from '@angular/core';
import { User } from 'src/app/_models/user';
import { ActivatedRoute } from '@angular/router';
import {
    NgxGalleryOptions,
    NgxGalleryImage,
    NgxGalleryAnimation
} from 'ngx-gallery';
import { TabsetComponent } from 'ngx-bootstrap';

@Component({
    selector: 'app-member-detail',
    templateUrl: './member-detail.component.html',
    styleUrls: ['./member-detail.component.css']
})
export class MemberDetailComponent implements OnInit {
    user: User;
    galleryOptions: NgxGalleryOptions[];
    galleryImages: NgxGalleryImage[];

    @ViewChild('memberTabs')
    memberTabs: TabsetComponent;

    constructor(
        private userService: UserService,
        private authService: AuthService,
        private alertify: AlertifyService,
        private route: ActivatedRoute
    ) { }

    ngOnInit() {
        this.route.data.subscribe(data => {
            this.user = data['user'];
        });

        this.route.queryParams.subscribe(params => {
            const selectedTab = params['tab'];
            this.memberTabs.tabs[selectedTab > 0 ? selectedTab : 0].active = true;
        });

        this.galleryOptions = [
            {
                width: '500px',
                height: '500px',
                imagePercent: 100,
                thumbnailsColumns: 4,
                imageAnimation: NgxGalleryAnimation.Slide,
                preview: false
            }
        ];

        this.galleryImages = this.getImages();
    }

    getImages() {
        const imageUrls = [];

        for (let i = 0; i < this.user.photos.length; i++) {
            imageUrls.push({
                small: this.user.photos[i].url,
                medium: this.user.photos[i].url,
                big: this.user.photos[i].url,
                description: this.user.photos[i].description
            });
        }

        return imageUrls;
    }

    sendLike(id: string) {
        this.userService
            .sendLike(this.authService.decodedToken.nameid, id)
            .subscribe(
                data => {
                    this.alertify.success('You have liked: ' + this.user.knownAs);
                },
                error => {
                    this.alertify.error(error);
                }
            );
    }

    selectTab(tabId: number) {
        this.memberTabs.tabs[tabId].active = true;
    }
}
