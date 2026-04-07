<%@ Page Title="Calendar" Language="VB" MasterPageFile="~/Site.master" AutoEventWireup="false" CodeFile="Calender.aspx.vb" Inherits="Calender" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">

    <div class="page-header">
        <div class="container-xl">
            <div class="row g-2 align-items-center">
                <div class="col">
                    <div class="page-pretitle">
                    <span id="pageAction">Notes</span>
                    </div>
                    <h2 class="page-title" id="pageTitle">Calendar</h2>
                </div>
            </div>
        </div>
    </div>

    <div class="page-body">
        <div class="container-xl" id="pageContent">
            <div class="row mb-3">
                <div class="col-lg-12 col-md-12 col-sm-12">
                    <div class="card">
                        <div class="card-header">
                            <h2 id="monthYear"></h2>
                        </div>
                        <div class="card-body">
                            <div class="row">
                               <div class="calendar" id="calendar"></div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <style>

        h2 {
            text-align: center;
        }

        .calendar {
            display: grid;
            grid-template-columns: repeat(7, 1fr);
            gap: 5px;
        }

        .day {
            border: 1px solid #ccc;
            min-height: 80px;
            padding: 5px;
            cursor: pointer;
            position: relative;
        }

        .day:hover {
            background: #a5a3a3;
        }

        .date {
            font-weight: bold;
        }

        .note {
            font-size: 12px;
            margin-top: 5px;
            color: #333;
        }

        .has-note {
            background: #a5a3a3;
        }
    </style>




    <script>
        let URIMETHOD = "/Methods/CalenderMethod.aspx";
        
    </script>
    <script type="text/javascript" src="/Scripts/Callender.js?<%= DateTime.Now.Ticks %>"></script>

</asp:Content>

