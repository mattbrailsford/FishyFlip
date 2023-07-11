﻿namespace FishyFlip.Models;

public class LikeRecord : ATRecord
{
    [JsonConstructor]
    public LikeRecord(Subject? subject, DateTime? createdAt)
    {
        this.Subject = subject;
        this.CreatedAt = createdAt ?? DateTime.Now;
        this.Type = Constants.FeedType.Like;
    }

    public Subject? Subject { get; }

    public DateTime CreatedAt { get; }
}
