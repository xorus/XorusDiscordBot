no documentation for now, sorry


but if you are scavenging for code, here is what this bot does:

- send planned reminders in a discord channel at a specific and recurring time
- /next discord command to display what's the next planned event (with an offset because reminders tend to be X hours before the actual event)
- assumes everything is local time (timezone must be set in environment, system or Dockerfile)


**config file example:**

```yaml
token: bottoken
register_to: guildid
reminders:
- name: Some event that occurs on mondays, thursdays and fridays with a reminder at 17:00:00
  cron: 0 0 17 ? * MON,THU,FRI
  guild: guildid
  in_next: true
  action: !message
    channel: channelid
    rotate: true
    strings:
    - reminder for <@&roleid>
    - another flavor of the same reminder for <@&roleid> that will be posted the next time
    next_pick: 0
  offset_seconds: 14400 # display the proper time in /next (the reminder is executed 4 hours before the actual event)
```
