<%@ Page Language="VB" AutoEventWireup="true" CodeFile="Faq.aspx.vb" Inherits="Faq" MasterPageFile="~/Site.master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Faq" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-header">
        <div class="container-xl">
            <div class="row g-2 align-items-center">
                <div class="col">
                    <h2 class="page-title">Frequently Asked Questions</h2>
                </div>
            </div>
        </div>
    </div>

    <div class="page-body">
        <div class="container-xl">
            <div class="card card-lg">
                <div class="card-body">
                    <div class="space-y-4">
                        <div>
                            <h2 class="mb-3">1. Introduction</h2>
                            <div id="faq-1" class="accordion" role="tablist" aria-multiselectable="true">
                                <div class="accordion-item">
                                    <div class="accordion-header" role="tab">
                                        <a href="#" class="accordion-button" data-bs-toggle="collapse" data-bs-target="#faq-welcome">Welcome to our service!</a>
                                    </div>

                                    <div id="faq-welcome" class="accordion-collapse collapse show" role="tabpanel" data-bs-parent="#faq-1">
                                        <div class="accordion-body pt-0">
                                            <div>
                                                <p>Lorem ipsum dolor sit amet, consectetur adipisicing elit. Accusantium alias dignissimos dolorum ea est eveniet, excepturi illum in iste iure maiores nemo recusandae rerum saepe sed, sunt totam! Explicabo, ipsa?</p>
                                                <p>Lorem ipsum dolor sit amet, consectetur adipisicing elit. Accusantium alias dignissimos dolorum ea est eveniet, excepturi illum in iste iure maiores nemo recusandae rerum saepe sed, sunt totam! Explicabo, ipsa?</p>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <div class="accordion-item">
                                    <div class="accordion-header" role="tab">
                                        <a href="#" class="accordion-button collapsed" data-bs-toggle="collapse" data-bs-target="#faq-who">Who are we?</a>
                                    </div>

                                    <div id="faq-who" class="accordion-collapse collapse" role="tabpanel" data-bs-parent="#faq-1">
                                        <div class="accordion-body pt-0">
                                            <div>
                                                <p>Lorem ipsum dolor sit amet, consectetur adipisicing elit. Accusantium alias dignissimos dolorum ea est eveniet, excepturi illum in iste iure maiores nemo recusandae rerum saepe sed, sunt totam! Explicabo, ipsa?</p>
                                                <p>Lorem ipsum dolor sit amet, consectetur adipisicing elit. Accusantium alias dignissimos dolorum ea est eveniet, excepturi illum in iste iure maiores nemo recusandae rerum saepe sed, sunt totam! Explicabo, ipsa?</p>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div>
                            <h2 class="mb-3">2. Functionality</h2>
                        </div>

                        <div>
                            <h2 class="mb-3">3. Payments</h2>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>