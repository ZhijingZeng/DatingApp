import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NgxGalleryAnimation, NgxGalleryImage, NgxGalleryOptions } from '@kolkov/ngx-gallery';
import { TabDirective, TabsetComponent } from 'ngx-bootstrap/tabs';
import { Member } from 'src/app/_models/member';
import { Message } from 'src/app/_models/message';
import { MembersService } from 'src/app/_services/members.service';
import { MessageService } from 'src/app/_services/message.service';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css']
})
export class MemberDetailComponent implements OnInit {
//we don't have access to the view until our component has been constructed

  @ViewChild('memberTabs',{static: true}) memberTabs?: TabsetComponent
  //we need to access it but we need to first get member(*ngIf member)
  //using root resolver, get the data before our root is activated

  //ViewChild does not like ngIf, so we need to delete it.
  //since we are getting member before this component is constructed
  //we actually do not need ngIf
  member: Member ={} as Member;
  galleryOptions: NgxGalleryOptions[] = [];
  galleryImages: NgxGalleryImage[]=[];
  activeTab?: TabDirective;
  messages: Message[] = [];

  constructor(private memberService: MembersService, private route: ActivatedRoute, private messageService: MessageService) { }
//we do not need to unsubscribe from activatedRoute
//https://stackoverflow.com/questions/41138081/do-i-have-to-unsubscribe-from-activatedroute-e-g-params-observables
  ngOnInit(): void {
    this.route.data.subscribe({
      next: data=>this.member = data['member']
    })
    this.route.queryParams.subscribe({
      next: params =>{
        params['tab'] && this.selectTab(params['tab'])
      }
    })
    this.galleryOptions = [
      {
        width: '500px',
        height : '500px',
        imagePercent:100,
        thumbnailsColumns:4,
        imageAnimation: NgxGalleryAnimation.Slide,
        preview: false
      }
    ]
    this.galleryImages = this.getImages();
  }
  getImages(){
    if(!this.member) return [];
    const imageUrls = [];
    for (const photo of this.member.photos){
      imageUrls.push({
        small: photo.url,
        medium: photo.url,
        big: photo.url
      })
    }
    return imageUrls
  }



  selectTab(heading: string){
    
    if(this.memberTabs){
      this.memberTabs.tabs.find(x =>x.heading == heading)!.active = true //! turn off typescript, we donot have type safety here
    }
  }

  loadMessages(){
    if(this.member){
      this.messageService.getMessageThread(this.member.userName).subscribe({
        next: messages => this.messages = messages
      })
    }
  }
  onTabActivated(data: TabDirective){
    this.activeTab = data;
    if(this.activeTab.heading === 'Messages'){
      this.loadMessages();
    }
  }
}
