# ARTCOMP 🎨
_"Discover Judge, Celebrate Art with ArtComp"_

An online space for artists to submit their work, and for judges to review and score the submissions. The platform streamlines the process of art judging, making it more efficient and accessible for artists, judges, and organizations.

## Features
- Artwork submissions: Competitors can join competitions and submit their artworks in various mediums like digital, gouache, watercolour and charcoal.
- Judging system: Judges can review and score the submissions of the competitors, helping to determine the winners.
- User management: Admins of ArtComp will manage the registration and authentication of competitors, judges, and the public. 
- Artwork display: The application displays all submissions in a user-friendly interface, allowing the public to view and vote on artworks easily.

## Design Considerations
### Sustainability
Each service runs separately and communicates with each other using lightweight mechanisms
If one service were to fail, other services can still work
Load balancing to  handle failures and unexpected events without disrupting the application's availability

### Aesthetics
Following our theme for ArtComp, we want the colour scheme to be pleasing to the eyes, and for the website to be easily recognisable as an art-sharing platform.

### User Experience
The content presented on our site is well-organised with a simple navigation structure and responsive design. Loading times are fast because we focus on speed and performance.

### Scalability
Hosted on the cloud, Google provides auto-scaling features that automatically add or remove resources based on changes in demand, further improving the scalability of cloud applications

### Security
User accounts dedicated for each profile (Administrator, Judge, Competitor), have their own login credentials.

## Architecture Diagram

## Instructions to set up and run the program
