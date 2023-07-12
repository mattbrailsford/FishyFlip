﻿// <copyright file="Repost.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

namespace FishyFlip.Models;

public class Repost : ATRecord
{
    public Repost(Cid cid, ATUri uri, DateTime createdAt)
    {
        this.Cid = cid;
        this.Uri = uri;
        this.CreatedAt = createdAt;
        this.Type = Constants.FeedType.Repost;
    }

    public Repost(CBORObject obj)
    {
        this.Cid = obj["subject"]["cid"].ToCid();
        this.Uri = new ATUri(obj["subject"]["uri"].AsString());
        this.CreatedAt = obj["createdAt"].ToDateTime();
        this.Type = Constants.FeedType.Repost;
    }

    public Cid? Cid { get; }

    public ATUri Uri { get; }

    public DateTime? CreatedAt { get; }
}